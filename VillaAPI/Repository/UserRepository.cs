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
        
        string jwtTokenId = $"JTI-{Guid.NewGuid()}";
        string accessToken = await GenerateAccessToken(user, jwtTokenId);

        string refreshToken = await CreateNewRefreshToken(user.Id, jwtTokenId);

        var tokenDTO = new TokenDTO() {
            AccessToken = accessToken,
            RefreshToken = refreshToken
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

    /// <summary>
    /// Generate an access token containing user's claims
    /// </summary>
    /// <param name="user"></param>
    /// <param name="jwtTokenId"></param>
    /// <returns>The new access token</returns>
    private async Task<string> GenerateAccessToken(ApplicationUser user, string jwtTokenId)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var userRole = roles.FirstOrDefault();

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor() {
            Subject = new ClaimsIdentity(
                new Claim[] {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, userRole),
                    new Claim(JwtRegisteredClaimNames.Jti, jwtTokenId),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id)
                }
            ),
            Expires = DateTime.UtcNow.AddMinutes(1),
            SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return tokenString;
    }

    /// <summary>
    /// Generate a new refresh token that has a unique value and save it to the database
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="tokenId"></param>
    /// <returns>The refresh token string</returns>
    private async Task<string> CreateNewRefreshToken(string userId, string tokenId)
    {
        var refreshToken = new RefreshToken() {
            UserId = userId,
            JwtTokenId = tokenId,
            Refresh_Token = $"{Guid.NewGuid()}-{Guid.NewGuid()}",
            IsValid = true,
            ExpiresAt = DateTime.UtcNow.AddMinutes(3)
        };

        await _db.RefreshTokens.AddAsync(refreshToken);
        await _db.SaveChangesAsync();

        return refreshToken.Refresh_Token;
    }

    private (bool isSuccessful, string userId, string tokenId) GetAccessTokenData(string accessToken)
    {
        try {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.ReadJwtToken(accessToken);
            var claims = jwt.Claims.ToList();

            string tokenId = claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
            string userId = claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
            return (true, userId, tokenId);
        } catch (Exception) {
            return (false, "", "");
        }
    }

    /// <summary>
    /// Refresh the access token
    /// </summary>
    /// <param name="tokenDTO">The object that contains the expired access token and the refresh token used to refresh</param>
    /// <returns>The new TokenDTO containing new access token and refresh token</returns>
    public async Task<TokenDTO?> RefreshAccessToken(TokenDTO tokenDTO)
    {
        // Find an existing refresh token
        var existingRefreshToken = await _db.RefreshTokens.FirstOrDefaultAsync(rt => rt.Refresh_Token == tokenDTO.RefreshToken);
        
        if (existingRefreshToken is null) {
            return null;
        }

        // Compare data from existing refresh token with the access token provided
        // If there is a mismatch, then consider it as a fraud
        var accessTokenData = GetAccessTokenData(tokenDTO.AccessToken);
        if (!accessTokenData.isSuccessful || accessTokenData.userId != existingRefreshToken.UserId 
                || accessTokenData.tokenId != existingRefreshToken.JwtTokenId) {
            existingRefreshToken.IsValid = false;
            await _db.SaveChangesAsync();
            return null;
        }

        // If just expired, then mark as invalid
        if (existingRefreshToken.ExpiresAt < DateTime.UtcNow) {
            existingRefreshToken.IsValid = false;
            await _db.SaveChangesAsync();
            return null;
        }

        // When someone tries to use an old refresh token (which is invalid), possible the refresh token is leaked
        // In this case, revoke all refresh tokens of the current login session of user -> force to re-login 
        if (!existingRefreshToken.IsValid) {
            var chainRecords = _db.RefreshTokens.Where(rt => rt.UserId == existingRefreshToken.UserId 
                && rt.JwtTokenId == existingRefreshToken.JwtTokenId && rt.IsValid);
            foreach (var record in chainRecords) {
                record.IsValid = false;
            }

            await _db.SaveChangesAsync();
            return null;
        }

        // Replace the existing refresh token with a new one that has updated expire date
        var newRefreshToken = await CreateNewRefreshToken(existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);

        // Revoke existing refresh token
        existingRefreshToken.IsValid = false;
        await _db.SaveChangesAsync();

        // Generate new access token
        var user = await _userManager.FindByIdAsync(existingRefreshToken.UserId);
        if (user is null) {
            return null;
        }

        string newAccessToken = await GenerateAccessToken(user, existingRefreshToken.JwtTokenId);

        return new TokenDTO() {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }
}
