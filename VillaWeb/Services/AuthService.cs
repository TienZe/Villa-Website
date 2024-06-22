using VillaWeb.Models;
using VillaWeb.Models.Dto;
using VillaWeb.Services.IServices;
using static VillaUtility.SD;

namespace VillaWeb.Services;
public class AuthService : BaseService, IAuthService
{
    protected readonly string _villaUrl;
    public AuthService(IHttpClientFactory httpClientFactory, IConfiguration configuration) : base(httpClientFactory)
    {
        _villaUrl = configuration["ServiceUrls:VillaAPI"];
    }

    public async Task<T?> LoginAsync<T>(LoginRequestDTO loginRequestDTO)
    {
        var apiRequest = new APIRequest() {
            ApiType = ApiType.POST,
            Url = _villaUrl + "/api/UserAuth/login",
            Data = loginRequestDTO
        };

        return await SendAsync<T>(apiRequest);
    }

    public async Task<T?> RegisterAsync<T>(RegisterationRequestDTO registerationRequestDTO)
    {
        var apiRequest = new APIRequest() {
            ApiType = ApiType.POST,
            Url = _villaUrl + "/api/UserAuth/register",
            Data = registerationRequestDTO
        };

        return await SendAsync<T>(apiRequest);
    }
}
