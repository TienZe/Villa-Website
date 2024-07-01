using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VillaUtility;
using VillaWeb.Infrastructures;
using VillaWeb.Models;
using VillaWeb.Models.Dto;
using VillaWeb.Services.IServices;

namespace VillaWeb.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    public IActionResult Login()
    {
        LoginRequestDTO model = new();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequestDTO model)
    {
        if (ModelState.IsValid) {
            var apiResponse = await _authService.LoginAsync<APIResponse>(model);

            if (apiResponse is not null) {
                if (apiResponse.IsSuccess) {
                    var tokenDTO = JsonConvert.DeserializeObject<TokenDTO>(Convert.ToString(apiResponse.Result));
                    string token = tokenDTO.AccessToken;
                    HttpContext.Session.SetString(SD.AccessTokenKey, token);

                    // Decode token (header and payload) to get user claims
                    var handler = new JwtSecurityTokenHandler();
                    var jwtSecurityToken = handler.ReadJwtToken(token);
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
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme, 
                        principal, 
                        new AuthenticationProperties {
                            IsPersistent = true
                        }
                    );

                    TempData["success"] = "You are logged in!";
                    return RedirectToAction("Index", "Home");
                } else {
                    ModelState.AddErrors(apiResponse.ErrorMessages);
                }
            } 
        }
        return View(model);
    }

    public IActionResult Register()
    {
        RegisterationRequestDTO model = new();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterationRequestDTO model)
    {
        if (ModelState.IsValid) {
            var apiResponse = await _authService.RegisterAsync<APIResponse>(model);
            if (apiResponse is not null) {
                if (apiResponse.IsSuccess) {
                    TempData["success"] = "Your account was created successfully!";
                    return RedirectToAction("Login");
                } else {
                    ModelState.AddErrors(apiResponse.ErrorMessages);
                }
            } 
        }
        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        // Default authentication scheme configured in Program.cs is Cookie Authentication
        // Delete cookie
        await HttpContext.SignOutAsync();
        // Remove the token stored in session
        HttpContext.Session.SetString(SD.AccessTokenKey, ""); 
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}
