using GestorHabitos.Application.Contracts;
using GestorHabitos.Application.DTO;
using GestorHabitos.Infrastructure.Interfaces;
using GestorHabitos.Infrastructure.Model;
using Mapster;

namespace GestorHabitos.Application.Servicios;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuario;

    public UsuarioService(IUsuarioRepository usuario)
    {
        _usuario = usuario;
    }

    public async Task<UsuarioDto> CreateAsync(UsuarioDto dto)
    {
        var entity = dto.Adapt<UsuarioModel>();
        await _usuario.CreateAsync(entity);
        return dto;
    }

    public async Task<List<UsuarioDto>> GetAllAsync()
    {
        var data = await _usuario.GetAllAsync();
        return data.Adapt<List<UsuarioDto>>();
    }

    public async Task<UsuarioDto> GetByIdAsync(int id)
    {
        var entity = await _usuario.GetByIdAsync(id);
        return entity.Adapt<UsuarioDto>();
    }

    public async Task<UsuarioDto> UpdateAsync(int id, UsuarioDto dto)
    {
        var entity = dto.Adapt<UsuarioModel>();
        await _usuario.UpdateAsync(id, entity);
        return dto;
    }

    public async Task DeleteAsync(int id)
    {
        await _usuario.DeleteAsync(id);
    }
}
