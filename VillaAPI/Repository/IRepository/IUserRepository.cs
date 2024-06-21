using VillaAPI.Models;
using VillaAPI.Models.Dto;

namespace VillaAPI.Repository.IRepository;
public interface IUserRepository
{
    bool IsUniqueUser(string username);
    Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
    Task<LocalUser> Register(RegisterationRequestDTO registerationRequestDTO); 

}
