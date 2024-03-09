using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace VillaAPI
{
    [ApiController]
    [Route("api/VillaAPI")]
    public class VillaAPIController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            return Ok(VillaStore.villaList);
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

            var villa = VillaStore.villaList.FirstOrDefault(villa => villa.Id == id);
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

            if (VillaStore.villaList.Find(villa => villa.Name.ToLower() == villaDTO.Name.ToLower()) != null) {
                ModelState.AddModelError("CustomError", "Villa name already exists");
                return BadRequest(ModelState);
            }

            villaDTO.Id = VillaStore.villaList.Select(villa => villa.Id).Max() + 1;
            VillaStore.villaList.Add(villaDTO);
            
            return CreatedAtAction(nameof(GetVilla), new { Id = villaDTO.Id }, villaDTO);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteVilla(int id)
        {
            if (id == 0) {
                return BadRequest();
            }

            var villa = VillaStore.villaList.FirstOrDefault(villa => villa.Id == id);
            if (villa is null) {
                return NotFound();
            }

            VillaStore.villaList.Remove(villa);
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

            var villa = VillaStore.villaList.Find(villa => villa.Id == id);
            if (villa is null) {
                return NotFound();
            }

            villa.Name = villaDTO.Name;
            villa.Occupancy = villaDTO.Occupancy;
            villa.Sqft = villaDTO.Sqft;

            return NoContent();
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdatePartiallyVilla(int id, [FromBody]JsonPatchDocument<VillaDTO> patchDoc)
        {
            if (patchDoc is null) {
                return BadRequest();
            }

            var villa = VillaStore.villaList.Find(villa => villa.Id == id);
            if (villa is null) {
                return NotFound();
            }

            patchDoc.ApplyTo(villa, ModelState);
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            return NoContent();
        }
    }

    
}
