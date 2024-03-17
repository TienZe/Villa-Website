using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using VillaAPI.Data;

namespace VillaAPI
{
    [ApiController]
    [Route("api/VillaAPI")]
    public class VillaAPIController : ControllerBase
    {
        private ApplicationDbContext _db;
        public VillaAPIController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            return Ok(_db.Villas.ToList());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDTO> GetVilla(int id)
        {
            if (id == 0) {
                return BadRequest();
            }

            var villa = _db.Villas.FirstOrDefault(villa => villa.Id == id);
            if (villa is null) {
                return NotFound();
            }

            return Ok(villa);
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDTO> CreateVilla([FromBody]VillaDTO villaDTO)
        {
            if (villaDTO is null) {
                return BadRequest();
            }

            if (villaDTO.Id > 0) {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            if (_db.Villas.Where(villa => villa.Name.ToLower() == villaDTO.Name.ToLower()).Any()) {
                ModelState.AddModelError("CustomError", "Villa name already exists");
                return BadRequest(ModelState);
            }

            var createdVilla = new Villa()
            {
                Name = villaDTO.Name,
                Occupancy = villaDTO.Occupancy,
                Sqft = villaDTO.Sqft,
                Rate = villaDTO.Rate,
                ImageUrl = villaDTO.ImageUrl,
                Amenity = villaDTO.Amenity,
                CreatedDate = DateTime.Now,
            };

            _db.Villas.Add(createdVilla);
            _db.SaveChanges();
            
            return CreatedAtAction(nameof(GetVilla), new { Id = villaDTO.Id }, villaDTO);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteVilla(int id)
        {
            if (id <= 0) {
                return BadRequest();
            }

            var villa = _db.Villas.Find(id);
            if (villa is null) {
                return NotFound();
            }

            _db.Villas.Remove(villa);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateVilla(int id, [FromBody]VillaDTO villaDTO)
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

            _db.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdatePartiallyVilla(int id, JsonPatchDocument<VillaDTO> patchDoc)
        {
            if (patchDoc is null) {
                return BadRequest();
            }

            var villa = _db.Villas.Find(id);
            if (villa is null) {
                return NotFound();
            }

            var villaDTO = new VillaDTO()
            {
                Name = villa.Name,
                Occupancy = villa.Occupancy,
                Sqft = villa.Sqft,
                Rate = villa.Rate,
                ImageUrl = villa.ImageUrl,
                Amenity = villa.Amenity
            };

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

            _db.SaveChanges();

            return NoContent();
        }
    }

    
}
