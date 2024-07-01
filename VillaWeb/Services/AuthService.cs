using VillaWeb.Models;
using VillaWeb.Models.Dto;
using VillaWeb.Services.IServices;
using static VillaUtility.SD;

namespace VillaWeb.Services;
public class AuthService : IAuthService
{
    protected readonly string _villaUrl;
    protected readonly IBaseService _baseService;
    public AuthService(IBaseService baseService, IConfiguration configuration) 
    {
        _baseService = baseService;
        _villaUrl = configuration["ServiceUrls:VillaAPI"];
    }

    public async Task<T?> LoginAsync<T>(LoginRequestDTO loginRequestDTO)
    {
        var apiRequest = new APIRequest() {
            ApiType = ApiType.POST,
            Url = _villaUrl + "/api/UserAuth/login",
            Data = loginRequestDTO
        };

        return await _baseService.SendAsync<T>(apiRequest, withBearerToken: false);
    }

    public async Task<T?> RegisterAsync<T>(RegisterationRequestDTO registerationRequestDTO)
    {
        var apiRequest = new APIRequest() {
            ApiType = ApiType.POST,
            Url = _villaUrl + "/api/UserAuth/register",
            Data = registerationRequestDTO
        };

        return await _baseService.SendAsync<T>(apiRequest, withBearerToken: false);
    }
}
