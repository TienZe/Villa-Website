using VillaWeb.Models.Dto;

namespace VillaWeb.Services.IServices;

public interface ISignInService
{
    Task SignInAsync(TokenDTO token);
    Task SignOutAsync();
}