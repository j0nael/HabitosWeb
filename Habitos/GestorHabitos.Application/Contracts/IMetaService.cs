using GestorHabitos.Application.DTO;

namespace GestorHabitos.Application.Contracts;

public interface IMetaService
{
    Task<MetaDto> CreateAsync(MetaDto dto);
    Task DeleteAsync(int id);
    Task<List<MetaDto>> GetAllAsync();
    Task<MetaDto> GetByIdAsync(int id);
    Task<MetaDto> UpdateAsync(int id, MetaDto dto);
}
