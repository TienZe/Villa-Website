using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using VillaAPI.Data;
using VillaAPI.Models;
using VillaAPI.Models.Dto;
using VillaAPI.Repository.IRepository;

namespace VillaAPI;
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _db;
    private readonly string _secretKey;
    public UserRepository(ApplicationDbContext db, IConfiguration config)
    {
        _db = db;
        _secretKey = config["ApiSettings:Secret"];
    }

    public bool IsUniqueUser(string username)
    {
        var user = _db.LocalUsers.FirstOrDefault(user => user.Username == username);
        return user is null;
    }

    public async Task<LoginResponseDTO?> Login(LoginRequestDTO loginRequestDTO)
    {
        var user = await _db.LocalUsers.FirstOrDefaultAsync(
            user => user.Username == loginRequestDTO.Username && user.Password == loginRequestDTO.Password);
        
        if (user is null) {
            return null;
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor() {
            Subject = new ClaimsIdentity(
                new Claim[] {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }
            ),
            Expires = DateTime.UtcNow.AddHours(10),
            SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        var loginResponse = new LoginResponseDTO() {
            User = user,
            Token = tokenString
        };

        return loginResponse;
    }   

    public async Task<LocalUser> Register(RegisterationRequestDTO registerationRequestDTO)
    {
        LocalUser user = new()
        {
            Name = registerationRequestDTO.Name,
            Username = registerationRequestDTO.Username,
            Password = registerationRequestDTO.Password,
            Role = registerationRequestDTO.Role
        };
        await _db.LocalUsers.AddAsync(user);
        await _db.SaveChangesAsync();

        user.Password = string.Empty;
        return user;
    }
}
