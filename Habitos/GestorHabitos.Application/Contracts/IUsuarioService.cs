using GestorHabitos.Application.DTO;

namespace GestorHabitos.Application.Contracts;

public interface IUsuarioService
{
    Task<UsuarioDto> CreateAsync(UsuarioDto dto);
    Task DeleteAsync(int id);
    Task<List<UsuarioDto>> GetAllAsync();
    Task<UsuarioDto> GetByIdAsync(int id);
    Task<UsuarioDto> UpdateAsync(int id, UsuarioDto dto);
}
