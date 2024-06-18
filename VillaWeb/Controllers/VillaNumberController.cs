using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VillaWeb.Models;
using VillaWeb.Models.Dto;
using VillaWeb.Services.IServices;

namespace VillaWeb;
public class VillaNumberController : Controller
{
    private readonly IMapper _mapper;
    private readonly IVillaNumberService _villaNumberService;

    public VillaNumberController(IMapper mapper, IVillaNumberService villaNumberService)
    {
        _mapper = mapper;
        _villaNumberService = villaNumberService;
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

}
