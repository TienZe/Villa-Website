using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using VillaWeb.Models;
using VillaWeb.Models.Dto;
using VillaWeb.Models.VM;
using VillaWeb.Services.IServices;
using VillaWeb.Infrastructures;
using Microsoft.AspNetCore.Authorization;

namespace VillaWeb;
public class VillaNumberController : Controller
{
    private readonly IMapper _mapper;
    private readonly IVillaNumberService _villaNumberService;
    private readonly IVillaService _villaService;
    public VillaNumberController(IMapper mapper, IVillaNumberService villaNumberService, IVillaService villaService)
    {
        _mapper = mapper;
        _villaNumberService = villaNumberService;
        _villaService = villaService;
    }

    public async Task<IActionResult> IndexVillaNumber()
    {
        List<VillaNumberDTO> list = new();
        var apiResponse = await _villaNumberService.GetAllAsync<APIResponse>();

        if (apiResponse is not null && apiResponse.IsSuccess) {
            list = JsonConvert.DeserializeObject<List<VillaNumberDTO>>(Convert.ToString(apiResponse.Result));
        }
        return View(list);
    }

    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateVillaNumber()
    {   
        VillaNumberCreateVM villaNumberVM = new();

        var villaApiResponse = await _villaService.GetAllAsync<APIResponse>();
        if (villaApiResponse is not null && villaApiResponse.IsSuccess) {
            var villas = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(villaApiResponse.Result));
            villaNumberVM.SelectListOfVillas = villas.Select(villa => new SelectListItem {
                Text = villa.Name,
                Value = villa.Id.ToString()
            });
        }
        
        return View(villaNumberVM);
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> CreateVillaNumber(VillaNumberCreateVM villaNumberCreateVM)
    {
        if (ModelState.IsValid) {
            var apiResponse = await _villaNumberService.CreateAsync<APIResponse>(villaNumberCreateVM.VillaNumber);
            if (apiResponse is not null) {
                if (apiResponse.IsSuccess) {
                    TempData["success"] = "Villa Number created successfully!";
                    return RedirectToAction(nameof(IndexVillaNumber));
                } else {
                    ModelState.AddModelErrors(apiResponse.ErrorMessages);
                }
            }
        }

        // Load the select list of villas
        var villaApiResponse = await _villaService.GetAllAsync<APIResponse>();
        if (villaApiResponse is not null && villaApiResponse.IsSuccess) {
            var villas = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(villaApiResponse.Result));
            villaNumberCreateVM.SelectListOfVillas = villas.Select(villa => new SelectListItem {
                Text = villa.Name,
                Value = villa.Id.ToString()
            });
        }
        return View(villaNumberCreateVM);
    }

    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateVillaNumber(int id)
    {   
        // Get the villa number with specified id
        APIResponse? villaNumberApiResponse = await _villaNumberService.GetAsync<APIResponse>(id);
        if (villaNumberApiResponse is not null && villaNumberApiResponse.IsSuccess) {
            VillaNumberDTO villaNumber = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(villaNumberApiResponse.Result));
            
            VillaNumberUpdateDTO updateDto = _mapper.Map<VillaNumberUpdateDTO>(villaNumber);
            VillaNumberUpdateVM villaNumberVM = new() {
                VillaNumber = updateDto
            };

            var villaApiResponse = await _villaService.GetAllAsync<APIResponse>();
            if (villaApiResponse is not null && villaApiResponse.IsSuccess) {
                var villas = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(villaApiResponse.Result));
                villaNumberVM.SelectListOfVillas = villas.Select(villa => new SelectListItem {
                    Text = villa.Name,
                    Value = villa.Id.ToString()
                });
            }
            return View(villaNumberVM);
        }
        return NotFound();    
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> UpdateVillaNumber(VillaNumberUpdateVM villaNumberUpdateVM)
    {
        if (ModelState.IsValid) {
            var apiResponse = await _villaNumberService.UpdateAsync<APIResponse>(villaNumberUpdateVM.VillaNumber);
            if (apiResponse is not null) {
                if (apiResponse.IsSuccess) {
                    TempData["success"] = "Villa Number updated successfully!";
                    return RedirectToAction(nameof(IndexVillaNumber));
                } else {
                    ModelState.AddModelErrors(apiResponse.ErrorMessages);
                }
            }
        }

        // Load the select list of villas
        var villaApiResponse = await _villaService.GetAllAsync<APIResponse>();
        if (villaApiResponse is not null && villaApiResponse.IsSuccess) {
            var villas = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(villaApiResponse.Result));
            villaNumberUpdateVM.SelectListOfVillas = villas.Select(villa => new SelectListItem {
                Text = villa.Name,
                Value = villa.Id.ToString()
            });
        }
        return View(villaNumberUpdateVM);
    }
    
    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> DeleteVillaNumber([FromForm]int id)
    {
        var apiResponse = await _villaNumberService.DeleteAsync<APIResponse>(id);

        if (apiResponse is not null && apiResponse.IsSuccess) {
            TempData["success"] = "Villa Number deleted successfully!";
            return RedirectToAction(nameof(IndexVillaNumber));
        }

        TempData["error"] = "Failed to delete this Villa Number!";
        return RedirectToAction(nameof(IndexVillaNumber));
    }
}
