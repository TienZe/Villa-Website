using System.Diagnostics;
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

            if (apiResponse is not null && apiResponse.IsSuccess) {
                var loginResponseDTO = JsonConvert.DeserializeObject<LoginResponseDTO>(Convert.ToString(apiResponse.Result));
                string token = loginResponseDTO.Token;
                HttpContext.Session.SetString(SD.SessionTokenKey, token);

                // Create claims
                var identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, loginResponseDTO.User.Username),
                    new Claim(ClaimTypes.NameIdentifier, loginResponseDTO.User.Id.ToString()),
                    new Claim(ClaimTypes.Role, loginResponseDTO.User.Role)
                }, CookieAuthenticationDefaults.AuthenticationScheme);

                // Create principal
                var principal = new ClaimsPrincipal(identity);
                
                // Sign in
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties {
                    IsPersistent = true
                });

                TempData["success"] = "You are logged in!";
                return RedirectToAction("Index", "Home");
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
                    ModelState.AddModelErrors(apiResponse.ErrorMessages);
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
        HttpContext.Session.SetString(SD.SessionTokenKey, ""); 
        return RedirectToAction("Index", "Home");
    }
}
