using GestorHabitos.Application.Contracts;
using GestorHabitos.Application.DTO;
using GestorHabitos.Infrastructure.Interfaces;
using GestorHabitos.Infrastructure.Model;
using Mapster;

namespace GestorHabitos.Application.Servicios;

public class RegistroService : IRegistroService
{
    private readonly IRegistroRepository _registro;

    public RegistroService(IRegistroRepository registro)
    {
        _registro = registro;
    }

    public async Task<RegistroDto> CreateAsync(RegistroDto dto)
    {
        var entity = dto.Adapt<RegistroModel>();
        await _registro.CreateAsync(entity);
        return dto;
    }

    public async Task<List<RegistroDto>> GetAllAsync()
    {
        var data = await _registro.GetAllAsync();
        return data.Adapt<List<RegistroDto>>();
    }

    public async Task<RegistroDto> GetByIdAsync(int id)
    {
        var entity = await _registro.GetByIdAsync(id);
        return entity.Adapt<RegistroDto>();
    }

    public async Task<RegistroDto> UpdateAsync(int id, RegistroDto dto)
    {
        var entity = dto.Adapt<RegistroModel>();
        await _registro.UpdateAsync(id, entity);
        return dto;
    }

    public async Task DeleteAsync(int id)
    {
        await _registro.DeleteAsync(id);
    }
}
