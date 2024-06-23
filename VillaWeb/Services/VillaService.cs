using System;
using VillaUtility;
using VillaWeb.Models;
using VillaWeb.Models.Dto;
using VillaWeb.Services.IServices;

namespace VillaWeb.Services;

public class VillaService : BaseService, IVillaService
{
    protected readonly string _villaUrl;
    public VillaService(IHttpClientFactory httpClientFactory, IConfiguration configuration) : base(httpClientFactory)
    {
        _villaUrl = configuration["ServiceUrls:VillaAPI"];
    }

    public Task<T?> CreateAsync<T>(VillaCreateDTO dto, string? token = null)
    {
        return SendAsync<T>(new APIRequest() {
            ApiType = SD.ApiType.POST,
            Url = _villaUrl + "/api/VillaAPI",
            Data = dto,
            Token = token,
        });
    }

    public Task<T?> DeleteAsync<T>(int id, string? token = null)
    {
        return SendAsync<T>(new APIRequest() {
            ApiType = SD.ApiType.DELETE,
            Url = _villaUrl + "/api/VillaAPI/" + id,
            Token = token,
        });
    }

    public Task<T?> GetAllAsync<T>(string? token = null)
    {
        return SendAsync<T>(new APIRequest() {
            ApiType = SD.ApiType.GET,
            Url = _villaUrl + "/api/VillaAPI",
            Token = token,
        });
    }

    public Task<T?> GetAsync<T>(int id, string? token = null)
    {
        return SendAsync<T>(new APIRequest() {
            ApiType = SD.ApiType.GET,
            Url = _villaUrl + "/api/VillaAPI/" + id,
            Token = token,           
        });
    }

    public Task<T> UpdateAsync<T>(VillaUpdateDTO dto, string? token = null)
    {
        return SendAsync<T>(new APIRequest() {
            ApiType = SD.ApiType.PUT,
            Url = _villaUrl + "/api/VillaAPI/" + dto.Id,
            Data = dto,
            Token = token,
        });    
    }
}