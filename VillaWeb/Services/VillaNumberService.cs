using System;
using VillaUtility;
using VillaWeb.Models;
using VillaWeb.Models.Dto;
using VillaWeb.Services.IServices;

namespace VillaWeb.Services;

public class VillaNumberService : BaseService, IVillaNumberService
{
    protected readonly string _villaUrl;
    public VillaNumberService(IHttpClientFactory httpClientFactory, IConfiguration configuration) : base(httpClientFactory)
    {
        _villaUrl = configuration["ServiceUrls:VillaAPI"];
    }

    public Task<T?> CreateAsync<T>(VillaNumberCreateDTO dto, string? token = null)
    {
        return SendAsync<T>(new APIRequest() {
            ApiType = SD.ApiType.POST,
            Url = _villaUrl + "/api/VillaNumber",
            Data = dto,
            Token = token,
        });
    }

    public Task<T?> DeleteAsync<T>(int id, string? token = null)
    {
        return SendAsync<T>(new APIRequest() {
            ApiType = SD.ApiType.DELETE,
            Url = _villaUrl + "/api/VillaNumber/" + id,
            Token = token,
        });
    }

    public Task<T?> GetAllAsync<T>(string? token = null)
    {
        return SendAsync<T>(new APIRequest() {
            ApiType = SD.ApiType.GET,
            Url = _villaUrl + "/api/VillaNumber",
            Token = token,
        });
    }

    public Task<T?> GetAsync<T>(int id, string? token = null)
    {
        return SendAsync<T>(new APIRequest() {
            ApiType = SD.ApiType.GET,
            Url = _villaUrl + "/api/VillaNumber/" + id,
            Token = token,
        });
    }

    public Task<T?> UpdateAsync<T>(VillaNumberUpdateDTO dto, string? token = null)
    {
        return SendAsync<T>(new APIRequest() {
            ApiType = SD.ApiType.PUT,
            Url = _villaUrl + "/api/VillaNumber/" + dto.VillaNo,
            Data = dto,
            Token = token,
        });    
    }
}