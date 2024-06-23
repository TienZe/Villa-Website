using VillaWeb.Models.Dto;

namespace VillaWeb.Services.IServices;

public interface IVillaService
{
    Task<T?> GetAllAsync<T>(string? token = null);
    Task<T?> GetAsync<T>(int id, string? token = null);
    Task<T?> CreateAsync<T>(VillaCreateDTO dto, string? token = null);
    Task<T?> UpdateAsync<T>(VillaUpdateDTO dto, string? token = null);
    Task<T?> DeleteAsync<T>(int id, string? token = null);
}