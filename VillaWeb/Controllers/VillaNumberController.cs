﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using VillaWeb.Models;
using VillaWeb.Models.Dto;
using VillaWeb.Models.VM;
using VillaWeb.Services.IServices;
using VillaWeb.Infrastructures;

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

    [HttpPost]
    public async Task<IActionResult> CreateVillaNumber(VillaNumberCreateVM villaNumberCreateVM)
    {
        if (ModelState.IsValid) {
            var apiResponse = await _villaNumberService.CreateAsync<APIResponse>(villaNumberCreateVM.VillaNumber);
            if (apiResponse is not null) {
                if (apiResponse.IsSuccess) {
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

}
