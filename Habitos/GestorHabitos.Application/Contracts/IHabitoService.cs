using GestorHabitos.Application.DTO;

namespace GestorHabitos.Application.Contracts;

public interface IHabitoService
{
    Task<HabitoDto> CreateAsync(HabitoDto dto);
    Task DeleteAsync(int id);
    Task<List<HabitoDto>> GetAllAsync();
    Task<HabitoDto> GetByIdAsync(int id);
    Task<HabitoDto> UpdateAsync(int id, HabitoDto dto);
}
