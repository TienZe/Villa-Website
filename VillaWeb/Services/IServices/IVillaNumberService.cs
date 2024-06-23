using VillaWeb.Models.Dto;

namespace VillaWeb.Services.IServices;

public interface IVillaNumberService
{
    Task<T?> GetAllAsync<T>(string? token = null);
    Task<T?> GetAsync<T>(int id, string? token = null);
    Task<T?> CreateAsync<T>(VillaNumberCreateDTO dto, string? token = null);
    Task<T?> UpdateAsync<T>(VillaNumberUpdateDTO dto, string? token = null);
    Task<T?> DeleteAsync<T>(int id, string? token = null);
}