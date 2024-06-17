using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

    public async Task<IActionResult> CreateVilla()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateVilla(VillaCreateDTO dto)
    {
        if (ModelState.IsValid) {
            var apiResponse = await _villaService.CreateAsync<APIResponse>(dto);

            if (apiResponse is not null && apiResponse.IsSuccess) {
                return RedirectToAction("IndexVilla");
            }
        }
        return View(dto);
    }
}
