using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
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

        protected APIResponse _response;
        
        public VillaAPIController(IMapper mapper, IVillaRepository repo)
        {
            _mapper = mapper;
            _repo = repo;
            _response = new();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillas()
        {
            IEnumerable<Villa> villas = await _repo.GetAllAsync();
            var villasDTO = _mapper.Map<IEnumerable<VillaDTO>>(villas);
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = villasDTO;
            return Ok(_response);
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
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Invalid Villa ID");
                return BadRequest(_response);
            }

            var villa = await _repo.GetAsync(villa => villa.Id == id);
            if (villa is null) {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Not found an villa with specified id");
                return NotFound();
            }
            
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = _mapper.Map<VillaDTO>(villa);
            return Ok(_response);
        }
        
        [Authorize(Roles = SD.Role.Admin)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody]VillaCreateDTO villaDTO)
        {
            if (villaDTO is null) {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest();
            }

            if ((await _repo.GetAsync(villa => villa.Name.ToLower() == villaDTO.Name.ToLower())) is not null) {
                ModelState.AddModelError("CustomError", "Villa name already exists");
                return BadRequest(ModelState);
            }

            var createdVilla = _mapper.Map<Villa>(villaDTO);
            createdVilla.CreatedDate = DateTime.Now;

            await _repo.CreateAsync(createdVilla);
            
            _response.StatusCode = HttpStatusCode.Created;
            _response.IsSuccess = true;
            _response.Result = _mapper.Map<VillaDTO>(createdVilla);
            return CreatedAtAction(nameof(GetVilla), new { Id = createdVilla.Id }, _response);
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
                return BadRequest();
            }

            var villa = await _repo.GetAsync(villa => villa.Id == id);
            if (villa is null) {
                return NotFound();
            }

            await _repo.RemoveAsync(villa);

            _response.StatusCode = HttpStatusCode.NoContent;
            _response.IsSuccess = true;
            return Ok(_response);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = SD.Role.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody]VillaUpdateDTO villaDTO)
        {
            if (villaDTO is null) {
                return BadRequest();
            }

            var villa = await _repo.GetAsync(villa => villa.Id == id, tracked: false);
            if (villa is null) {
                return NotFound();
            }

            villa.Name = villaDTO.Name;
            villa.Occupancy = villaDTO.Occupancy;
            villa.Sqft = villaDTO.Sqft;
            villa.Rate = villaDTO.Rate;
            villa.ImageUrl = villaDTO.ImageUrl;
            villa.Amenity = villaDTO.Amenity;
            villa.Details = villaDTO.Details;

            await _repo.UpdateAsync(villa);

            _response.StatusCode = HttpStatusCode.NoContent;
            _response.IsSuccess = true;
            return Ok(_response);
        }

        [HttpPatch("{id:int}")]
        [Authorize(Roles = SD.Role.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdatePartiallyVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDoc)
        {
            if (patchDoc is null) {
                return BadRequest();
            }

            var villa = await _repo.GetAsync(villa => villa.Id == id, tracked: false);
            if (villa is null) {
                return NotFound();
            }

            var villaDTO = _mapper.Map<VillaUpdateDTO>(villa);

            patchDoc.ApplyTo(villaDTO, ModelState);
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            villa.Name = villaDTO.Name;
            villa.Occupancy = villaDTO.Occupancy;
            villa.Sqft = villaDTO.Sqft;
            villa.Rate = villaDTO.Rate;
            villa.ImageUrl = villaDTO.ImageUrl;
            villa.Amenity = villaDTO.Amenity;

            await _repo.UpdateAsync(villa);

            return NoContent();
        }
    }

    
}
