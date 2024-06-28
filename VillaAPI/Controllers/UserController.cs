using System.Net;
using Microsoft.AspNetCore.Mvc;
using VillaAPI.Infrastructure;
using VillaAPI.Migrations;
using VillaAPI.Models;
using VillaAPI.Models.Dto;
using VillaAPI.Repository.IRepository;
using VillaUtility;
namespace VillaAPI.Controllers;

[ApiController]
[Route("api/UserAuth")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepo;
    public UserController(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody]LoginRequestDTO loginRequestDTO)
    {
        LoginResponseDTO? loginResponse = await _userRepo.Login(loginRequestDTO);
        if (loginResponse is null)
        {
            return BadRequest(APIResponse.BadRequest(
                errorMessages: new string[] { "Invalid username or password" }
            ));
        }

        return Ok(APIResponse.Ok(loginResponse));
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody]RegisterationRequestDTO registerationRequestDTO)
    {
        bool isUniqueUsername = _userRepo.IsUniqueUser(registerationRequestDTO.Username);
        if (!isUniqueUsername) {
            return BadRequest(APIResponse.BadRequest(
                errorMessages: new string[] { "Username already exists" }
            ));
        }

        var role = registerationRequestDTO.Role;
        if (role is not null) {
            var roleList = SD.Role.GetRoles();
            if (!roleList.Contains(role)) {
                return BadRequest(APIResponse.BadRequest(
                    errorMessages: new string[] { "Invalid role" }
                ));
            }
        } else {
            registerationRequestDTO.Role = SD.Role.User;
        }

        try {
            await _userRepo.Register(registerationRequestDTO);
        } catch (IdentityException e) {
            return BadRequest(APIResponse.BadRequest(
                errorMessages: e.Errors
            ));
        }

        return Ok(APIResponse.Ok());
    }
}
