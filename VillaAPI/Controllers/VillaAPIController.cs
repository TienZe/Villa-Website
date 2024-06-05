using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VillaAPI.Data;

namespace VillaAPI
{
    [ApiController]
    [Route("api/VillaAPI")]
    public class VillaAPIController : ControllerBase
    {
        private ApplicationDbContext _db;
        private IMapper _mapper;
        public VillaAPIController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            IEnumerable<Villa> villas = await _db.Villas.ToListAsync();
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

            var villa = await _db.Villas.FirstOrDefaultAsync(villa => villa.Id == id);
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

            if (_db.Villas.Where(villa => villa.Name.ToLower() == villaDTO.Name.ToLower()).Any()) {
                ModelState.AddModelError("CustomError", "Villa name already exists");
                return BadRequest(ModelState);
            }

            var createdVilla = _mapper.Map<Villa>(villaDTO);
            createdVilla.CreatedDate = DateTime.Now;

            await _db.Villas.AddAsync(createdVilla);
            await _db.SaveChangesAsync();
            
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

            var villa = _db.Villas.Find(id);
            if (villa is null) {
                return NotFound();
            }

            _db.Villas.Remove(villa);
            await _db.SaveChangesAsync();

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

            var villa = _db.Villas.Find(id);
            if (villa is null) {
                return NotFound();
            }

            villa.Name = villaDTO.Name;
            villa.Occupancy = villaDTO.Occupancy;
            villa.Sqft = villaDTO.Sqft;
            villa.Rate = villaDTO.Rate;
            villa.ImageUrl = villaDTO.ImageUrl;
            villa.Amenity = villaDTO.Amenity;

            await _db.SaveChangesAsync();
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

            var villa = _db.Villas.Find(id);
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

            await _db.SaveChangesAsync();

            return NoContent();
        }
    }

    
}
