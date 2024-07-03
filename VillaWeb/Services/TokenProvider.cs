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
        _httpContextAccessor.HttpContext?.Response.Cookies.Delete(SD.AccessTokenCookieName);
        _httpContextAccessor.HttpContext?.Response.Cookies.Delete(SD.RefreshTokenCookieName);
    }

    public TokenDTO? GetToken()
    {
        string? accessToken = null;
        string? refreshToken = null;

        _httpContextAccessor.HttpContext?.Request.Cookies.TryGetValue(SD.AccessTokenCookieName, out accessToken);
        _httpContextAccessor.HttpContext?.Request.Cookies.TryGetValue(SD.RefreshTokenCookieName, out refreshToken);

        if (accessToken is null || refreshToken is null) {
            return null;
        }

        return new TokenDTO { 
            AccessToken = accessToken,
            RefreshToken = refreshToken
        }; 
    }

    public void SetToken(TokenDTO tokenDTO)
    {
        var cookieOption = new CookieOptions {
            Expires = DateTime.UtcNow.AddMonths(1),
        };
        _httpContextAccessor.HttpContext?.Response.Cookies.Append(SD.AccessTokenCookieName, tokenDTO.AccessToken, cookieOption);
        _httpContextAccessor.HttpContext?.Response.Cookies.Append(SD.RefreshTokenCookieName, tokenDTO.RefreshToken, cookieOption);
    }
}