using GestorHabitos.Application.DTO;

namespace GestorHabitos.Application.Contracts;

public interface IRegistroService
{
    Task<RegistroDto> CreateAsync(RegistroDto dto);
    Task DeleteAsync(int id);
    Task<List<RegistroDto>> GetAllAsync();
    Task<RegistroDto> GetByIdAsync(int id);
    Task<RegistroDto> UpdateAsync(int id, RegistroDto dto);
}
