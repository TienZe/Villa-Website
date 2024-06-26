using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VillaUtility;
using VillaWeb.Infrastructures;
using VillaWeb.Models;
using VillaWeb.Models.Dto;
using VillaWeb.Services.IServices;

namespace VillaWeb.Controllers;

public class VillaController : Controller
{
    private readonly IVillaService _villaService;
    private readonly IMapper _mapper;
    public VillaController(IVillaService villaService, IMapper mapper)
    {
        _villaService = villaService;
        _mapper = mapper;
    }

    public async Task<IActionResult> IndexVilla()
    {
        List<VillaDTO> list = new();
        var apiResponse = await _villaService.GetAllAsync<APIResponse>();

        if (apiResponse is not null && apiResponse.IsSuccess) {
            list = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(apiResponse.Result));
        }
        return View(list);
    }

    [Authorize(Roles = SD.Role.Admin)]
    public IActionResult CreateVilla()
    {
        return View();
    }

    [Authorize(Roles = SD.Role.Admin)]
    [HttpPost]
    public async Task<IActionResult> CreateVilla(VillaCreateDTO dto)
    {
        if (ModelState.IsValid) {
            var token = HttpContext.Session.GetString(SD.SessionTokenKey);
            var apiResponse = await _villaService.CreateAsync<APIResponse>(dto, token);

            if (apiResponse is not null ) {
                if (apiResponse.IsSuccess) {
                    TempData["success"] = "Villa created successfully!";
                    return RedirectToAction(nameof(IndexVilla));
                }
                // Error occurred
                ModelState.AddErrors(apiResponse.ErrorMessages);
            }
        }
        return View(dto);
    }
    [Authorize(Roles = SD.Role.Admin)]
    public async Task<IActionResult> UpdateVilla(int id)
    {
        var apiResponse = await _villaService.GetAsync<APIResponse>(id);
        if (apiResponse is not null && apiResponse.IsSuccess) {
            VillaDTO villaDTO = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(apiResponse.Result));
            VillaUpdateDTO villaUpdateDTO = _mapper.Map<VillaUpdateDTO>(villaDTO);
            return View(villaUpdateDTO);
        }

        return NotFound();
    }
    [Authorize(Roles = SD.Role.Admin)]
    [HttpPost]
    public async Task<IActionResult> UpdateVilla(VillaUpdateDTO dto)
    {
        if (ModelState.IsValid) {
            var token = HttpContext.Session.GetString(SD.SessionTokenKey);
            var apiResponse = await _villaService.UpdateAsync<APIResponse>(dto, token);

            if (apiResponse is not null) {
                if (apiResponse.IsSuccess) {
                    TempData["success"] = "Villa updated successfully!";
                    return RedirectToAction(nameof(IndexVilla));
                }
                // Error occurred
                ModelState.AddErrors(apiResponse.ErrorMessages);
            }
        }
        return View(dto);
    }
    [Authorize(Roles = SD.Role.Admin)]
    [HttpPost]
    public async Task<IActionResult> DeleteVilla([FromForm]int id)
    {
        var token = HttpContext.Session.GetString(SD.SessionTokenKey);
        var apiResponse = await _villaService.DeleteAsync<APIResponse>(id, token);

        if (apiResponse is not null) {
            if (apiResponse.IsSuccess) {
                TempData["success"] = "Villa deleted successfully!";
                return RedirectToAction(nameof(IndexVilla));
            }
        }

        TempData["error"] = "Failed to delete this Villa!";
        return RedirectToAction(nameof(IndexVilla));
    }
}
