using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using VillaAPI.Data;
using VillaAPI.Infrastructure;
using VillaAPI.Models;
using VillaAPI.Models.Dto;
using VillaAPI.Repository.IRepository;
using VillaUtility;

namespace VillaAPI;
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _db;
    private readonly string _secretKey;

    private readonly UserManager<ApplicationUser> _userManager;

    public UserRepository(ApplicationDbContext db, IConfiguration config
        , UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _secretKey = config["ApiSettings:Secret"];
        _userManager = userManager;
    }

    public bool IsUniqueUser(string username)
    {
        var user = _db.LocalUsers.FirstOrDefault(user => user.Username == username);
        return user is null;
    }

    public async Task<TokenDTO?> Login(LoginRequestDTO loginRequestDTO)
    {
        var user = await _userManager.FindByNameAsync(loginRequestDTO.Username);
        var isValidCredentials = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);
        if (user is null || !isValidCredentials) {
            return null;
        }
        
        var roles = await _userManager.GetRolesAsync(user);
        var userRole = roles.FirstOrDefault();

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor() {
            Subject = new ClaimsIdentity(
                new Claim[] {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, userRole)
                }
            ),
            Expires = DateTime.UtcNow.AddHours(10),
            SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        var tokenDTO = new TokenDTO() {
            AccessToken = tokenString
        };

        return tokenDTO;
    }   

    public async Task<ApplicationUser> Register(RegisterationRequestDTO registerationRequestDTO)
    {
        var newUser = new ApplicationUser() {
            Name = registerationRequestDTO.Name,
            UserName = registerationRequestDTO.Username,
        };

        var result = await _userManager.CreateAsync(newUser, registerationRequestDTO.Password);

        if (!result.Succeeded) {
            throw new IdentityException("Failed to create a new user", result.Errors.Select(error => error.Description));
        }

        var add2RoleResult = await _userManager.AddToRoleAsync(newUser, registerationRequestDTO.Role ?? SD.Role.User);

        if (!add2RoleResult.Succeeded) {
            throw new IdentityException("Failed to assign the specified role to user", result.Errors.Select(error => error.Description));
        }

        return newUser;
    }
}
