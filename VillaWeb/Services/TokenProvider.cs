using VillaUtility;
using VillaWeb.Models.Dto;
using VillaWeb.Services.IServices;

namespace VillaWeb.Services;

public class TokenProvider : ITokenProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public TokenProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public void ClearToken()
    {
        _httpContextAccessor.HttpContext?.Response.Cookies.Delete(SD.AccessTokenKey);
    }

    public TokenDTO? GetToken()
    {
        string? accessToken = null;
        _httpContextAccessor.HttpContext?.Request.Cookies.TryGetValue(SD.AccessTokenKey, out accessToken);

        if (accessToken is null) {
            return null;
        }

        return new TokenDTO { AccessToken = accessToken }; 
    }

    public void SetToken(TokenDTO tokenDTO)
    {
        var cookieOption = new CookieOptions {
            Expires = DateTime.UtcNow.AddHours(10),
        };
        _httpContextAccessor.HttpContext?.Response.Cookies.Append(SD.AccessTokenKey, tokenDTO.AccessToken, cookieOption);
    }
}