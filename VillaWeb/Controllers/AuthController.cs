using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VillaWeb.Infrastructures;
using VillaWeb.Models;
using VillaWeb.Models.Dto;
using VillaWeb.Services.IServices;

namespace VillaWeb.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly ISignInService _signInService;
    public AuthController(IAuthService authService, ISignInService signInService)
    {
        _authService = authService;
        _signInService = signInService;
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

                    await _signInService.SignInAsync(tokenDTO);

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
        _signInService.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}
