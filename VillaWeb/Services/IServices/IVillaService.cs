using VillaWeb.Models.Dto;

namespace VillaWeb.Services.IServices;

public interface IVillaService
{
    Task<T> GetAllAsync<T>();
    Task<T> GetAsync<T>(int id);
    Task<T> CreateAsync<T>(VillaCreateDTO dto);
    Task<T> UpdateAsync<T>(int id, VillaUpdateDTO dto);
    Task<T> DeleteAsync<T>(int id);
}