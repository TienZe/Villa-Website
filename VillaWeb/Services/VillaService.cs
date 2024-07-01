using System;
using VillaUtility;
using VillaWeb.Infrastructures;
using VillaWeb.Models;
using VillaWeb.Models.Dto;
using VillaWeb.Services.IServices;

namespace VillaWeb.Services;

public class VillaService : IVillaService
{
    protected readonly string _villaUrl;
    protected readonly IBaseService _baseService;
    public VillaService(IBaseService baseService, IConfiguration configuration) 
    {
        _baseService = baseService;
        _villaUrl = configuration["ServiceUrls:VillaAPI"];
    }

    public async Task<T?> CreateAsync<T>(VillaCreateDTO dto)
    {
        return await _baseService.SendAsync<T>(new APIRequest() {
            ApiType = SD.ApiType.POST,
            Url = _villaUrl + "/api/VillaAPI",
            Data = dto,
            ContentType = ContentTypes.MultipartFormData,
        });
    }

    public async Task<T?> DeleteAsync<T>(int id)
    {
        return await _baseService.SendAsync<T>(new APIRequest() {
            ApiType = SD.ApiType.DELETE,
            Url = _villaUrl + "/api/VillaAPI/" + id,
        });
    }

    public async Task<T?> GetAllAsync<T>()
    {
        return await _baseService.SendAsync<T>(new APIRequest() {
            ApiType = SD.ApiType.GET,
            Url = _villaUrl + "/api/VillaAPI",
        });
    }

    public async Task<T?> GetAsync<T>(int id)
    {
        return await _baseService.SendAsync<T>(new APIRequest() {
            ApiType = SD.ApiType.GET,
            Url = _villaUrl + "/api/VillaAPI/" + id,
        });
    }

    public async Task<T?> UpdateAsync<T>(VillaUpdateDTO dto)
    {
        return await _baseService.SendAsync<T>(new APIRequest() {
            ApiType = SD.ApiType.PUT,
            Url = _villaUrl + "/api/VillaAPI/" + dto.Id,
            Data = dto,
            ContentType = ContentTypes.MultipartFormData,
        });    
    }
}