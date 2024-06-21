using System.Net;
using Microsoft.AspNetCore.Mvc;
using VillaAPI.Models;
using VillaAPI.Models.Dto;
using VillaAPI.Repository.IRepository;

namespace VillaAPI.Controllers;

[ApiController]
[Route("api/UserAuth")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepo;
    protected APIResponse _response;
    public UserController(IUserRepository userRepo)
    {
        _userRepo = userRepo;
        _response = new();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody]LoginRequestDTO loginRequestDTO)
    {
        LoginResponseDTO? loginResponse = await _userRepo.Login(loginRequestDTO);
        if (loginResponse is null)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.ErrorMessages.Add("Invalid username or password");
            _response.IsSuccess = false;
            return BadRequest(_response);
        }

        _response.StatusCode = HttpStatusCode.OK;
        _response.IsSuccess = true;
        _response.Result = loginResponse;
        return Ok(_response);
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody]RegisterationRequestDTO registerationRequestDTO)
    {
        bool isUniqueUsername = _userRepo.IsUniqueUser(registerationRequestDTO.Username);
        if (!isUniqueUsername)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.ErrorMessages.Add("Username already exists");
            _response.IsSuccess = false;
            return BadRequest(_response);
        }
        await _userRepo.Register(registerationRequestDTO);

        _response.StatusCode = HttpStatusCode.OK;
        _response.IsSuccess = true;
        return Ok(_response);
    }
}
