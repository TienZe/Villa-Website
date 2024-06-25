using System.Net;
using System.Runtime.CompilerServices;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using VillaAPI.Models;
using VillaAPI.Models.Dto;
using VillaAPI.Repository.IRepository;
using VillaUtility;

namespace VillaAPI.Controllers;

[Route("api/VillaNumber")]
[ApiController]
public class VillaNumberController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IVillaNumberRepository _repo;
    private readonly IVillaRepository _villaRepo;

    public VillaNumberController(IVillaNumberRepository repo, IVillaRepository villaRepo, IMapper mapper)
    {
        _repo = repo;
        _villaRepo = villaRepo;
        _mapper = mapper;
    }

    [HttpGet]
    [ResponseCache(Duration = 30)]
    public async Task<ActionResult<APIResponse>> GetVillaNumbers()
    {
        IEnumerable<VillaNumber> villaNumbers = await _repo.GetAllAsync(tracked: false
            , includeProperties: "Villa");
        var villaNumberDtos = _mapper.Map<IEnumerable<VillaNumberDTO>>(villaNumbers);
        return Ok(APIResponse.Ok(villaNumberDtos));
    }
    [HttpGet("{villaNo:int}")]
    public async Task<ActionResult<APIResponse>> GetVillaNumber(int villaNo)
    {
        var villaNumber = await _repo.GetAsync(villaNumber => villaNumber.VillaNo == villaNo
            , includeProperties: "Villa");
        if (villaNumber is null) {
            return NotFound(APIResponse.NotFound(
                errorMessages: new string[] { "Villa number not found" }
            ));
        }

        return Ok(APIResponse.Ok(_mapper.Map<VillaNumberDTO>(villaNumber)));
    }

    [Authorize(Roles = SD.Role.Admin)]
    [HttpPost]
    public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody]VillaNumberCreateDTO dto)
    {
        if ((await _repo.GetAsync(villaNumber => villaNumber.VillaNo == dto.VillaNo)) != null) {
            return BadRequest(APIResponse.BadRequest(
                errorMessages: new string[] { "Villa number already exists" }
            ));
        }

        var villa = await _villaRepo.GetAsync(villa => villa.Id == dto.VillaID);
        if (villa is null) {
            return BadRequest(APIResponse.BadRequest(
                errorMessages: new string[] { "Villa with specied Id does not exist" }
            ));
        }

        var villaNumber = _mapper.Map<VillaNumber>(dto);
        await _repo.CreateAsync(villaNumber);

        var apiResponse = APIResponse.Created( _mapper.Map<VillaNumberDTO>(villaNumber));
        return CreatedAtAction(nameof(GetVillaNumber), new { villaNo = villaNumber.VillaNo }, apiResponse);
    }
    
    [Authorize(Roles = SD.Role.Admin)]
    [HttpPut("{villaNo:int}")]
    public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int villaNo, [FromBody]VillaNumberUpdateDTO dto)
    {
        var villaNumber = await _repo.GetAsync(villaNumber => villaNumber.VillaNo == villaNo, tracked: false);
        if (villaNumber is null) {
            return NotFound(APIResponse.NotFound(
                errorMessages: new string[] { "Villa number not found" }
            ));
        }

        var villa = await _villaRepo.GetAsync(villa => villa.Id == dto.VillaID);
        if (villa is null) {
            return BadRequest(APIResponse.BadRequest(
                errorMessages: new string[] { "Villa with specied Id does not exist" }
            ));
        }

        villaNumber.SpecialDetails = dto.SpecialDetails;
        villaNumber.VillaID = dto.VillaID;

        await _repo.UpdateAsync(villaNumber);

        return Ok(APIResponse.NoContent());
    }
        
    [Authorize(Roles = SD.Role.Admin)]
    [HttpDelete("{villaNo:int}")]
    public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int villaNo)
    {
        var villaNumber = await _repo.GetAsync(villaNumber => villaNumber.VillaNo == villaNo);
        if (villaNumber is null) {
            return NotFound(APIResponse.NotFound(
                errorMessages: new string[] { "Villa number not found" }
            ));
        }

        await _repo.RemoveAsync(villaNumber);

        return Ok(APIResponse.NoContent());
    }
    
    [Authorize(Roles = SD.Role.Admin)]
    [HttpPatch("{villaNo:int}")]
    public async Task<ActionResult<APIResponse>> UpdatePartiallyVillaNumber(int villaNo, [FromBody]JsonPatchDocument<VillaNumberUpdateDTO> patchDoc)
    {
        if (patchDoc is null) {
            return BadRequest(APIResponse.BadRequest());
        }

        var villaNumber = await _repo.GetAsync(villaNumber => villaNumber.VillaNo == villaNo, tracked: false);
        if (villaNumber is null) {
            return NotFound(APIResponse.NotFound());
        }

        var villaNumberUpdateDto = _mapper.Map<VillaNumberUpdateDTO>(villaNumber);
        patchDoc.ApplyTo(villaNumberUpdateDto, ModelState);
        if (!ModelState.IsValid) {
            return BadRequest(APIResponse.BadRequest(
                validationErrors: ModelState.ToErrorDictionary()
            ));
        }

        villaNumber.SpecialDetails = villaNumberUpdateDto.SpecialDetails;

        await _repo.UpdateAsync(villaNumber);

        return Ok(APIResponse.NoContent());
    }
}