using System.Net;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using VillaAPI.Infrastructure;
using VillaAPI.Models;
using VillaAPI.Models.Dto;
using VillaAPI.Repository.IRepository;
using VillaUtility;

namespace VillaAPI
{
    [ApiController]
    [Route("api/VillaAPI")]
    public class VillaAPIController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IVillaRepository _repo;
        private readonly FileService _fileService;


        public VillaAPIController(IMapper mapper, IVillaRepository repo, FileService fileService)
        {
            _mapper = mapper;
            _repo = repo;
            _fileService = fileService;
        }

        [HttpGet]
        [ResponseCache(Duration = 30)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillas([FromQuery]string? search, [FromQuery]int? pageIndex)
        {
            pageIndex = pageIndex ?? 1;

            PaginatedList<Villa> paginatedVillas;
            if (string.IsNullOrEmpty(search)) {
                paginatedVillas = await _repo.GetAllAsync(pageIndex: pageIndex.Value);
            } else {
                // SQL Collation is defaultly case-insensitive
                paginatedVillas = await _repo.GetAllAsync(villa => villa.Name.Contains(search)
                    , pageIndex: pageIndex.Value);
            }
            
            // Add pagination metadata to response header
            string paginationMetaData = JsonSerializer.Serialize(paginatedVillas.MetaData);
            Response.Headers.Add("X-Pagination", paginationMetaData);

            var villasDTO = _mapper.Map<IEnumerable<VillaDTO>>(paginatedVillas);

            return Ok(APIResponse.Ok(villasDTO));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            if (id == 0) {
                return BadRequest(APIResponse.BadRequest(
                    errorMessages: new string[] { "Invalid Villa ID" }
                ));
            }

            var villa = await _repo.GetAsync(villa => villa.Id == id);
            if (villa is null) {
                return NotFound(APIResponse.NotFound(
                    errorMessages: new string[] { "Not found an villa with specified id" }
                ));
            }
            
            return Ok(APIResponse.Ok(_mapper.Map<VillaDTO>(villa)));
        }
        
        [Authorize(Roles = SD.Role.Admin)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDTO>> CreateVilla([FromForm]VillaCreateDTO villaDTO)
        {
            if (villaDTO is null) {
                return BadRequest(APIResponse.BadRequest(
                    errorMessages: new string[] { "Villa object is null" }
                ));
            }

            if ((await _repo.GetAsync(villa => villa.Name.ToLower() == villaDTO.Name.ToLower())) is not null) {
                ModelState.AddModelError(nameof(villaDTO.Name), "Villa name already exists");
                return BadRequest(APIResponse.BadRequest(
                    validationErrors: ModelState.ToErrorDictionary()
                ));
            }

            var createdVilla = _mapper.Map<Villa>(villaDTO);

            if (villaDTO.Image is not null) {
                // Upload image to server
                try {
                    string filePath = _fileService.UploadImage(villaDTO.Image, ConfigConstant.VillaUploadImagePath);
                    
                    createdVilla.ImageLocalPath = filePath;
                    string baseUrl = $"{Request.Scheme}://{Request.Host.Value}{Request.PathBase}";
                    createdVilla.ImageUrl = $"{baseUrl}/{filePath}";
                } catch (Exception e) {
                    return BadRequest(APIResponse.BadRequest(
                        errorMessages: new string[] { e.Message }
                    ));
                }
            } else {
                createdVilla.ImageUrl = ConfigConstant.DefaultVillaImageUrl;
            }

            createdVilla.CreatedDate = DateTime.Now;
            await _repo.CreateAsync(createdVilla);
            
            var apiResponse = APIResponse.Created(_mapper.Map<VillaDTO>(createdVilla));
            return CreatedAtAction(nameof(GetVilla), new { Id = createdVilla.Id }, apiResponse);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = SD.Role.Admin)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
        {
            if (id <= 0) {
                return BadRequest(APIResponse.BadRequest(
                    errorMessages: new string[] { "Invalid villa ID" }
                ));
            }

            var villa = await _repo.GetAsync(villa => villa.Id == id);
            if (villa is null) {
                return NotFound(APIResponse.NotFound(
                    errorMessages: new string[] { "Villa not found" }
                ));
            }
            
            // Delete villa's image if exists
            if (!string.IsNullOrEmpty(villa.ImageLocalPath)) {
                _fileService.DeleteFile(villa.ImageLocalPath);
            }

            await _repo.RemoveAsync(villa);

            return Ok(APIResponse.NoContent());
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = SD.Role.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromForm]VillaUpdateDTO villaDTO)
        {
            if (villaDTO is null) {
                return BadRequest(APIResponse.BadRequest());
            }

            var villa = await _repo.GetAsync(villa => villa.Id == id, tracked: false);
            if (villa is null) {
                return NotFound(APIResponse.NotFound());
            }
            
            // Update villa props
            villa.Name = villaDTO.Name;
            villa.Occupancy = villaDTO.Occupancy;
            villa.Sqft = villaDTO.Sqft;
            villa.Rate = villaDTO.Rate;
            villa.Amenity = villaDTO.Amenity;
            villa.Details = villaDTO.Details;

            if (villaDTO.NewImage is not null) {
                string? oldImagePath = villa.ImageLocalPath;

                // Upload new image
                try {
                    string filePath = _fileService.UploadImage(villaDTO.NewImage, ConfigConstant.VillaUploadImagePath);
                    
                    villa.ImageLocalPath = filePath;
                    string baseUrl = $"{Request.Scheme}://{Request.Host.Value}{Request.PathBase}";
                    villa.ImageUrl = $"{baseUrl}/{filePath}";
                } catch (Exception e) {
                    return BadRequest(APIResponse.BadRequest(
                        errorMessages: new string[] { e.Message }
                    ));
                }

                // Delete old image if exists
                if (!string.IsNullOrEmpty(oldImagePath)) {
                    _fileService.DeleteFile(oldImagePath);
                }
            }

            await _repo.UpdateAsync(villa);

            return Ok(APIResponse.NoContent());
        }

        [HttpPatch("{id:int}")]
        [Authorize(Roles = SD.Role.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdatePartiallyVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDoc)
        {
            if (patchDoc is null) {
                return BadRequest(APIResponse.BadRequest());
            }

            var villa = await _repo.GetAsync(villa => villa.Id == id, tracked: false);
            if (villa is null) {
                return NotFound(APIResponse.NotFound());
            }

            var villaDTO = _mapper.Map<VillaUpdateDTO>(villa);

            patchDoc.ApplyTo(villaDTO, ModelState);
            if (!ModelState.IsValid) {
                return BadRequest(APIResponse.BadRequest(
                    validationErrors: ModelState.ToErrorDictionary()
                ));
            }

            villa.Name = villaDTO.Name;
            villa.Occupancy = villaDTO.Occupancy;
            villa.Sqft = villaDTO.Sqft;
            villa.Rate = villaDTO.Rate;
            villa.Amenity = villaDTO.Amenity;

            await _repo.UpdateAsync(villa);

            return NoContent();
        }
    }

    
}
