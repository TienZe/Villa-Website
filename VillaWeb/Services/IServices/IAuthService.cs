using VillaWeb.Models.Dto;

namespace VillaWeb.Services.IServices;
public interface IAuthService
{
    Task<T?> LoginAsync<T>(LoginRequestDTO loginRequestDTO);
    Task<T?> RegisterAsync<T>(RegisterationRequestDTO registerationRequestDTO);
}
