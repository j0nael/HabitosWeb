using GestorHabitos.Application.Contracts;
using GestorHabitos.Application.DTO;
using GestorHabitos.Infrastructure.Interfaces;
using GestorHabitos.Infrastructure.Model;
using Mapster;

namespace GestorHabitos.Application.Servicios;

public class HabitoService : IHabitoService
{
    private readonly IHabitoRepository _habito;

    public HabitoService(IHabitoRepository habito)
    {
        _habito = habito;
    }

    public async Task<HabitoDto> CreateAsync(HabitoDto dto)
    {
        var entity = dto.Adapt<HabitoModel>();
        await _habito.CreateAsync(entity);
        return dto;
    }

    public async Task<List<HabitoDto>> GetAllAsync()
    {
        var data = await _habito.GetAllAsync();
        return data.Adapt<List<HabitoDto>>();
    }

    public async Task<HabitoDto> GetByIdAsync(int id)
    {
        var entity = await _habito.GetByIdAsync(id);
        return entity.Adapt<HabitoDto>();
    }

    public async Task<HabitoDto> UpdateAsync(int id, HabitoDto dto)
    {
        var entity = dto.Adapt<HabitoModel>();
        await _habito.UpdateAsync(id, entity);
        return dto;
    }

    public async Task DeleteAsync(int id)
    {
        await _habito.DeleteAsync(id);
    }
}
