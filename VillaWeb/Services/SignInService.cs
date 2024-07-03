using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using VillaWeb.Models.Dto;
using VillaWeb.Services.IServices;

namespace VillaWeb.Services;

public class SignInService : ISignInService
{
    private readonly ITokenProvider _tokenProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public SignInService(ITokenProvider tokenProvider, IHttpContextAccessor httpContextAccessor)
    {
        _tokenProvider = tokenProvider;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task SignInAsync(TokenDTO tokenDTO)
    {
        // Store the token in cookie
        _tokenProvider.SetToken(tokenDTO);

        // Decode token (header and payload) to get user claims
        string accessToken = tokenDTO.AccessToken;
        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(accessToken);
        var claims = jwtSecurityToken.Claims.ToList();

        string? userId = claims.FirstOrDefault(c => c.Type == "nameid")?.Value ?? "";
        string? userName = claims.FirstOrDefault(c => c.Type == "unique_name")?.Value ?? "";
        string? userRole = claims.FirstOrDefault(c => c.Type == "role")?.Value ?? "";

        // Create claims
        var identity = new ClaimsIdentity(
            new[] {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, userRole)
            }, 
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        // Create principal
        var principal = new ClaimsPrincipal(identity);
        
        // Sign in
        await _httpContextAccessor.HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme, 
            principal, 
            new AuthenticationProperties {
                IsPersistent = true
            }
        );
    }

    public async Task SignOutAsync()
    {
        // Default authentication scheme configured in Program.cs is Cookie Authentication
        // Delete authentication cookie
        await _httpContextAccessor.HttpContext.SignOutAsync();

        // Remove the token stored in cookie
        _tokenProvider.ClearToken();
    }
}