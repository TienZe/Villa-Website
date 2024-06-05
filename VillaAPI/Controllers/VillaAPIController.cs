using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VillaAPI.Data;
using VillaAPI.Repository.IRepository;

namespace VillaAPI
{
    [ApiController]
    [Route("api/VillaAPI")]
    public class VillaAPIController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IVillaRepository _repo;
        
        public VillaAPIController(IMapper mapper, IVillaRepository repo)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            IEnumerable<Villa> villas = await _repo.GetAllAsync();
            var villasDTO = _mapper.Map<IEnumerable<VillaDTO>>(villas);
            return Ok(villasDTO);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDTO>> GetVilla(int id)
        {
            if (id == 0) {
                return BadRequest();
            }

            var villa = await _repo.GetAsync(villa => villa.Id == id);
            if (villa is null) {
                return NotFound();
            }

            return Ok(_mapper.Map<VillaDTO>(villa));
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody]VillaCreateDTO villaDTO)
        {
            if (villaDTO is null) {
                return BadRequest();
            }

            if ((await _repo.GetAsync(villa => villa.Name.ToLower() == villaDTO.Name.ToLower())) is not null) {
                ModelState.AddModelError("CustomError", "Villa name already exists");
                return BadRequest(ModelState);
            }

            var createdVilla = _mapper.Map<Villa>(villaDTO);
            createdVilla.CreatedDate = DateTime.Now;

            await _repo.CreateAsync(createdVilla);
            
            return CreatedAtAction(nameof(GetVilla), new { Id = createdVilla.Id }, createdVilla);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteVilla(int id)
        {
            if (id <= 0) {
                return BadRequest();
            }

            var villa = await _repo.GetAsync(villa => villa.Id == id);
            if (villa is null) {
                return NotFound();
            }

            await _repo.RemoveAsync(villa);

            return NoContent();
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateVilla(int id, [FromBody]VillaUpdateDTO villaDTO)
        {
            if (villaDTO is null) {
                return BadRequest();
            }

            var villa = await _repo.GetAsync(villa => villa.Id == id);
            if (villa is null) {
                return NotFound();
            }

            villa.Name = villaDTO.Name;
            villa.Occupancy = villaDTO.Occupancy;
            villa.Sqft = villaDTO.Sqft;
            villa.Rate = villaDTO.Rate;
            villa.ImageUrl = villaDTO.ImageUrl;
            villa.Amenity = villaDTO.Amenity;

            await _repo.SaveAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdatePartiallyVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDoc)
        {
            if (patchDoc is null) {
                return BadRequest();
            }

            var villa = await _repo.GetAsync(villa => villa.Id == id);
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

            await _repo.SaveAsync();

            return NoContent();
        }
    }

    
}
