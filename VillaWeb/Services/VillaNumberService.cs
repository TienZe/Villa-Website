using System;
using VillaUtility;
using VillaWeb.Models;
using VillaWeb.Models.Dto;
using VillaWeb.Services.IServices;

namespace VillaWeb.Services;

public class VillaNumberService : BaseService, IVillaNumberService
{
    protected readonly string _villaUrl;
    public VillaNumberService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider
        , IConfiguration configuration) 
            : base(httpClientFactory, tokenProvider)
    {
        _villaUrl = configuration["ServiceUrls:VillaAPI"];
    }

    public Task<T?> CreateAsync<T>(VillaNumberCreateDTO dto)
    {
        return SendAsync<T>(new APIRequest() {
            ApiType = SD.ApiType.POST,
            Url = _villaUrl + "/api/VillaNumber",
            Data = dto,
        });
    }

    public Task<T?> DeleteAsync<T>(int id)
    {
        return SendAsync<T>(new APIRequest() {
            ApiType = SD.ApiType.DELETE,
            Url = _villaUrl + "/api/VillaNumber/" + id,
        });
    }

    public Task<T?> GetAllAsync<T>()
    {
        return SendAsync<T>(new APIRequest() {
            ApiType = SD.ApiType.GET,
            Url = _villaUrl + "/api/VillaNumber",
        });
    }

    public Task<T?> GetAsync<T>(int id)
    {
        return SendAsync<T>(new APIRequest() {
            ApiType = SD.ApiType.GET,
            Url = _villaUrl + "/api/VillaNumber/" + id,
        });
    }

    public Task<T?> UpdateAsync<T>(VillaNumberUpdateDTO dto)
    {
        return SendAsync<T>(new APIRequest() {
            ApiType = SD.ApiType.PUT,
            Url = _villaUrl + "/api/VillaNumber/" + dto.VillaNo,
            Data = dto,
        });    
    }
}