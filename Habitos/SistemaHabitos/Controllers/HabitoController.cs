using GestorHabitos.Application.DTO;
using GestorHabitos.Infrastructure.Data;
using GestorHabitos.Infrastructure.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SistemaHabitos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HabitoController : ControllerBase
{
    private readonly AppDbContext _context;

    public HabitoController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _context.Habitos
            .Include(h => h.CategoriaHabito)
            .Include(h => h.Usuario)
            .ToListAsync();

        var list = data.Select(h => new
        {
            h.Id,
            h.Nombre,
            h.Descripcion,
            h.Frecuencia,
            h.HoraRecordatorio,
            h.Activo,
            h.FechaCreacion,
            h.Color,
            h.Icono,
            h.UsuarioId,
            UsuarioNombre = h.Usuario.Nombre,
            h.CategoriaHabitoId,
            CategoriaNombre = h.CategoriaHabito.Nombre
        });
        return Ok(list);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var h = await _context.Habitos
            .Include(h => h.CategoriaHabito)
            .Include(h => h.Usuario)
            .FirstOrDefaultAsync(h => h.Id == id);
        if (h == null) return NotFound("Hábito no encontrado.");
        return Ok(new HabitoDto
        {
            Id = h.Id,
            Nombre = h.Nombre,
            Descripcion = h.Descripcion,
            Frecuencia = h.Frecuencia,
            HoraRecordatorio = h.HoraRecordatorio,
            Activo = h.Activo,
            FechaCreacion = h.FechaCreacion,
            Color = h.Color,
            Icono = h.Icono,
            UsuarioId = h.UsuarioId,
            CategoriaHabitoId = h.CategoriaHabitoId
        });
    }

    [HttpGet("usuario/{usuarioId}")]
    public async Task<IActionResult> GetByUsuario(int usuarioId)
    {
        var data = await _context.Habitos
            .Include(h => h.CategoriaHabito)
            .Where(h => h.UsuarioId == usuarioId)
            .ToListAsync();

        var list = data.Select(h => new
        {
            h.Id,
            h.Nombre,
            h.Descripcion,
            h.Frecuencia,
            h.HoraRecordatorio,
            h.Activo,
            h.FechaCreacion,
            h.Color,
            h.Icono,
            h.UsuarioId,
            h.CategoriaHabitoId,
            CategoriaNombre = h.CategoriaHabito.Nombre
        });
        return Ok(list);
    }

    [HttpPost]
    public async Task<IActionResult> Create(HabitoDto dto)
    {
        var entity = new HabitoModel
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            Frecuencia = dto.Frecuencia,
            HoraRecordatorio = dto.HoraRecordatorio,
            Activo = dto.Activo,
            FechaCreacion = DateTime.Now,
            Color = dto.Color,
            Icono = dto.Icono,
            UsuarioId = dto.UsuarioId,
            CategoriaHabitoId = dto.CategoriaHabitoId
        };
        _context.Habitos.Add(entity);
        await _context.SaveChangesAsync();
        dto.Id = entity.Id;
        return Ok(dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, HabitoDto dto)
    {
        var entity = await _context.Habitos.FindAsync(id);
        if (entity == null) return NotFound("Hábito no encontrado.");
        entity.Nombre = dto.Nombre;
        entity.Descripcion = dto.Descripcion;
        entity.Frecuencia = dto.Frecuencia;
        entity.HoraRecordatorio = dto.HoraRecordatorio;
        entity.Activo = dto.Activo;
        entity.Color = dto.Color;
        entity.Icono = dto.Icono;
        entity.UsuarioId = dto.UsuarioId;
        entity.CategoriaHabitoId = dto.CategoriaHabitoId;
        await _context.SaveChangesAsync();
        return Ok(dto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _context.Habitos
            .Include(h => h.Registros)
            .FirstOrDefaultAsync(h => h.Id == id);
        if (entity == null) return NotFound("Hábito no encontrado.");
        if (entity.Registros.Any())
            return BadRequest("No se puede eliminar, el hábito tiene registros o metas asociadas.");
        _context.Habitos.Remove(entity);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
