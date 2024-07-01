using VillaWeb.Models.Dto;

namespace VillaWeb.Services.IServices;

public interface ITokenProvider
{
    void SetToken(TokenDTO tokenDTO);
    TokenDTO? GetToken();
    void ClearToken();
}