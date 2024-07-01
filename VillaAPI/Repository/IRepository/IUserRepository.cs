using VillaAPI.Models;
using VillaAPI.Models.Dto;

namespace VillaAPI.Repository.IRepository;
public interface IUserRepository
{
    bool IsUniqueUser(string username);
    Task<TokenDTO?> Login(LoginRequestDTO loginRequestDTO);
    Task<ApplicationUser> Register(RegisterationRequestDTO registerationRequestDTO); 

}
