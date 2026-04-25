using GestorHabitos.Application.DTO;

namespace GestorHabitos.Application.Contracts;

public interface ICategoriaHabitoService
{
    Task<CategoriaHabitoDto> CreateAsync(CategoriaHabitoDto dto);
    Task DeleteAsync(int id);
    Task<List<CategoriaHabitoDto>> GetAllAsync();
    Task<CategoriaHabitoDto> GetByIdAsync(int id);
    Task<CategoriaHabitoDto> UpdateAsync(int id, CategoriaHabitoDto dto);
}
