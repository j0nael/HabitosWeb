using GestorHabitos.Application.Contracts;
using GestorHabitos.Application.DTO;
using GestorHabitos.Infrastructure.Interfaces;
using GestorHabitos.Infrastructure.Model;
using Mapster;

namespace GestorHabitos.Application.Servicios;

public class MetaService : IMetaService
{
    private readonly IMetaRepository _meta;

    public MetaService(IMetaRepository meta)
    {
        _meta = meta;
    }

    public async Task<MetaDto> CreateAsync(MetaDto dto)
    {
        var entity = dto.Adapt<MetaModel>();
        await _meta.CreateAsync(entity);
        return dto;
    }

    public async Task<List<MetaDto>> GetAllAsync()
    {
        var data = await _meta.GetAllAsync();
        return data.Adapt<List<MetaDto>>();
    }

    public async Task<MetaDto> GetByIdAsync(int id)
    {
        var entity = await _meta.GetByIdAsync(id);
        return entity.Adapt<MetaDto>();
    }

    public async Task<MetaDto> UpdateAsync(int id, MetaDto dto)
    {
        var entity = dto.Adapt<MetaModel>();
        await _meta.UpdateAsync(id, entity);
        return dto;
    }

    public async Task DeleteAsync(int id)
    {
        await _meta.DeleteAsync(id);
    }
}
