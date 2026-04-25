using GestorHabitos.Application.Contracts;
using GestorHabitos.Application.DTO;
using GestorHabitos.Infrastructure.Interfaces;
using GestorHabitos.Infrastructure.Model;
using Mapster;

namespace GestorHabitos.Application.Servicios;

public class CategoriaHabitoService : ICategoriaHabitoService
{
    private readonly ICategoriaHabitoRepository _categoria;

    public CategoriaHabitoService(ICategoriaHabitoRepository categoria)
    {
        _categoria = categoria;
    }

    public async Task<CategoriaHabitoDto> CreateAsync(CategoriaHabitoDto dto)
    {
        var entity = dto.Adapt<CategoriaHabitoModel>();
        await _categoria.CreateAsync(entity);
        return dto;
    }

    public async Task<List<CategoriaHabitoDto>> GetAllAsync()
    {
        var data = await _categoria.GetAllAsync();
        return data.Adapt<List<CategoriaHabitoDto>>();
    }

    public async Task<CategoriaHabitoDto> GetByIdAsync(int id)
    {
        var entity = await _categoria.GetByIdAsync(id);
        return entity.Adapt<CategoriaHabitoDto>();
    }

    public async Task<CategoriaHabitoDto> UpdateAsync(int id, CategoriaHabitoDto dto)
    {
        var entity = dto.Adapt<CategoriaHabitoModel>();
        await _categoria.UpdateAsync(id, entity);
        return dto;
    }

    public async Task DeleteAsync(int id)
    {
        await _categoria.DeleteAsync(id);
    }
}
