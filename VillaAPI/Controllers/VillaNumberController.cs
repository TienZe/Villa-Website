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
    protected APIResponse _response;

    public VillaNumberController(IVillaNumberRepository repo, IVillaRepository villaRepo, IMapper mapper)
    {
        _repo = repo;
        _villaRepo = villaRepo;
        _mapper = mapper;
        _response = new();
    }

    [HttpGet]
    public async Task<ActionResult<APIResponse>> GetVillaNumbers()
    {
        IEnumerable<VillaNumber> villaNumbers = await _repo.GetAllAsync(tracked: false
            , includeProperties: "Villa");
        var villaNumberDtos = _mapper.Map<IEnumerable<VillaNumberDTO>>(villaNumbers);
        _response.StatusCode = HttpStatusCode.OK;
        _response.IsSuccess = true;
        _response.Result = villaNumberDtos;
        return Ok(_response);
    }
    [HttpGet("{villaNo:int}")]
    public async Task<ActionResult<APIResponse>> GetVillaNumber(int villaNo)
    {
        var villaNumber = await _repo.GetAsync(villaNumber => villaNumber.VillaNo == villaNo
            , includeProperties: "Villa");
        if (villaNumber is null) {
            _response.StatusCode = HttpStatusCode.NotFound;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Villa number not found");
            return NotFound(_response);
        }

        _response.StatusCode = HttpStatusCode.OK;
        _response.IsSuccess = true;
        _response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
        return Ok(_response);
    }

    [Authorize(Roles = SD.Role.Admin)]
    [HttpPost]
    public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody]VillaNumberCreateDTO dto)
    {
        if ((await _repo.GetAsync(villaNumber => villaNumber.VillaNo == dto.VillaNo)) != null) {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Villa number already exists");
            return BadRequest(_response);
        }

        var villa = await _villaRepo.GetAsync(villa => villa.Id == dto.VillaID);
        if (villa is null) {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Villa with specied Id does not exist");
            return BadRequest(_response);
        }

        var villaNumber = _mapper.Map<VillaNumber>(dto);
        await _repo.CreateAsync(villaNumber);

        _response.StatusCode = HttpStatusCode.Created;
        _response.IsSuccess = true;
        _response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
        return CreatedAtAction(nameof(GetVillaNumber), new { villaNo = villaNumber.VillaNo },_response);
    }
    
    [Authorize(Roles = SD.Role.Admin)]
    [HttpPut("{villaNo:int}")]
    public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int villaNo, [FromBody]VillaNumberUpdateDTO dto)
    {
        var villaNumber = await _repo.GetAsync(villaNumber => villaNumber.VillaNo == villaNo, tracked: false);
        if (villaNumber is null) {
            _response.StatusCode = HttpStatusCode.NotFound;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Villa number not found");
            return NotFound(_response);
        }

        var villa = await _villaRepo.GetAsync(villa => villa.Id == dto.VillaID);
        if (villa is null) {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Villa with specied Id does not exist");
            return BadRequest(_response);
        }

        villaNumber.SpecialDetails = dto.SpecialDetails;
        villaNumber.VillaID = dto.VillaID;

        await _repo.UpdateAsync(villaNumber);

        _response.StatusCode = HttpStatusCode.NoContent;
        _response.IsSuccess = true;
        return Ok(_response);
    }
        
    [Authorize(Roles = SD.Role.Admin)]
    [HttpDelete("{villaNo:int}")]
    public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int villaNo)
    {
        var villaNumber = await _repo.GetAsync(villaNumber => villaNumber.VillaNo == villaNo);
        if (villaNumber is null) {
            _response.StatusCode = HttpStatusCode.NotFound;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Villa number not found");
            return NotFound(_response);
        }

        await _repo.RemoveAsync(villaNumber);

        _response.StatusCode = HttpStatusCode.NoContent;
        _response.IsSuccess = true;
        return Ok(_response);
    }
    
    [Authorize(Roles = SD.Role.Admin)]
    [HttpPatch("{villaNo:int}")]
    public async Task<ActionResult<APIResponse>> UpdatePartiallyVillaNumber(int villaNo, [FromBody]JsonPatchDocument<VillaNumberUpdateDTO> patchDoc)
    {
        if (patchDoc is null) {
            return BadRequest();
        }

        var villaNumber = await _repo.GetAsync(villaNumber => villaNumber.VillaNo == villaNo, tracked: false);
        if (villaNumber is null) {
            return NotFound();
        }

        var villaNumberUpdateDto = _mapper.Map<VillaNumberUpdateDTO>(villaNumber);
        patchDoc.ApplyTo(villaNumberUpdateDto, ModelState);
        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }

        villaNumber.SpecialDetails = villaNumberUpdateDto.SpecialDetails;

        await _repo.UpdateAsync(villaNumber);

        _response.StatusCode = HttpStatusCode.NoContent;
        _response.IsSuccess = true;
        return Ok(_response);
    }
}