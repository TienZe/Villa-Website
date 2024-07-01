using System;
using VillaUtility;
using VillaWeb.Models;
using VillaWeb.Models.Dto;
using VillaWeb.Services.IServices;

namespace VillaWeb.Services;

public class VillaNumberService : IVillaNumberService
{
    protected readonly string _villaUrl;
    protected readonly IBaseService _baseService;
    public VillaNumberService(IBaseService baseService, IConfiguration configuration) 
    {
        _baseService = baseService;
        _villaUrl = configuration["ServiceUrls:VillaAPI"];
    }

    public async Task<T?> CreateAsync<T>(VillaNumberCreateDTO dto)
    {
        return await _baseService.SendAsync<T>(new APIRequest() {
            ApiType = SD.ApiType.POST,
            Url = _villaUrl + "/api/VillaNumber",
            Data = dto,
        });
    }

    public async Task<T?> DeleteAsync<T>(int id)
    {
        return await _baseService.SendAsync<T>(new APIRequest() {
            ApiType = SD.ApiType.DELETE,
            Url = _villaUrl + "/api/VillaNumber/" + id,
        });
    }

    public async Task<T?> GetAllAsync<T>()
    {
        return await _baseService.SendAsync<T>(new APIRequest() {
            ApiType = SD.ApiType.GET,
            Url = _villaUrl + "/api/VillaNumber",
        });
    }

    public async Task<T?> GetAsync<T>(int id)
    {
        return await _baseService.SendAsync<T>(new APIRequest() {
            ApiType = SD.ApiType.GET,
            Url = _villaUrl + "/api/VillaNumber/" + id,
        });
    }

    public async Task<T?> UpdateAsync<T>(VillaNumberUpdateDTO dto)
    {
        return await _baseService.SendAsync<T>(new APIRequest() {
            ApiType = SD.ApiType.PUT,
            Url = _villaUrl + "/api/VillaNumber/" + dto.VillaNo,
            Data = dto,
        });    
    }
}