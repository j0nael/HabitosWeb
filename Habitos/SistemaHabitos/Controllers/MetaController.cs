using GestorHabitos.Application.DTO;
using GestorHabitos.Infrastructure.Data;
using GestorHabitos.Infrastructure.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SistemaHabitos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MetaController : ControllerBase
{
    private readonly AppDbContext _context;

    public MetaController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _context.Metas
            .Include(m => m.Habito)
            .Include(m => m.Usuario)
            .ToListAsync();

        var list = data.Select(m => new
        {
            m.Id,
            m.Nombre,
            m.Descripcion,
            m.HabitoId,
            HabitoNombre = m.Habito.Nombre,
            m.UsuarioId,
            UsuarioNombre = m.Usuario.Nombre,
            m.FechaInicio,
            m.FechaFin,
            m.ObjetivoDias,
            m.Completada,
            Progreso = _context.Registros
                .Count(r => r.HabitoId == m.HabitoId && r.Completado && r.Fecha >= m.FechaInicio && r.Fecha <= m.FechaFin)
        });
        return Ok(list);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var m = await _context.Metas
            .Include(m => m.Habito)
            .Include(m => m.Usuario)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (m == null) return NotFound("Meta no encontrada.");

        var progreso = await _context.Registros
            .CountAsync(r => r.HabitoId == m.HabitoId && r.Completado && r.Fecha >= m.FechaInicio && r.Fecha <= m.FechaFin);

        return Ok(new
        {
            m.Id,
            m.Nombre,
            m.Descripcion,
            m.HabitoId,
            HabitoNombre = m.Habito.Nombre,
            m.UsuarioId,
            m.FechaInicio,
            m.FechaFin,
            m.ObjetivoDias,
            m.Completada,
            Progreso = progreso
        });
    }

    [HttpGet("usuario/{usuarioId}")]
    public async Task<IActionResult> GetByUsuario(int usuarioId)
    {
        var data = await _context.Metas
            .Include(m => m.Habito)
            .Where(m => m.UsuarioId == usuarioId)
            .ToListAsync();

        var list = data.Select(m => new
        {
            m.Id,
            m.Nombre,
            m.Descripcion,
            m.HabitoId,
            HabitoNombre = m.Habito.Nombre,
            m.FechaInicio,
            m.FechaFin,
            m.ObjetivoDias,
            m.Completada,
            Progreso = _context.Registros
                .Count(r => r.HabitoId == m.HabitoId && r.Completado && r.Fecha >= m.FechaInicio && r.Fecha <= m.FechaFin)
        });
        return Ok(list);
    }

    [HttpPost]
    public async Task<IActionResult> Create(MetaDto dto)
    {
        var entity = new MetaModel
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            HabitoId = dto.HabitoId,
            UsuarioId = dto.UsuarioId,
            FechaInicio = dto.FechaInicio,
            FechaFin = dto.FechaFin,
            ObjetivoDias = dto.ObjetivoDias,
            Completada = false
        };
        _context.Metas.Add(entity);
        await _context.SaveChangesAsync();
        dto.Id = entity.Id;
        return Ok(dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, MetaDto dto)
    {
        var entity = await _context.Metas.FindAsync(id);
        if (entity == null) return NotFound("Meta no encontrada.");
        entity.Nombre = dto.Nombre;
        entity.Descripcion = dto.Descripcion;
        entity.HabitoId = dto.HabitoId;
        entity.UsuarioId = dto.UsuarioId;
        entity.FechaInicio = dto.FechaInicio;
        entity.FechaFin = dto.FechaFin;
        entity.ObjetivoDias = dto.ObjetivoDias;
        entity.Completada = dto.Completada;
        await _context.SaveChangesAsync();
        return Ok(dto);
    }

    [HttpPut("{id}/completar")]
    public async Task<IActionResult> Completar(int id)
    {
        var entity = await _context.Metas.FindAsync(id);
        if (entity == null) return NotFound("Meta no encontrada.");
        entity.Completada = true;
        await _context.SaveChangesAsync();
        return Ok(new { mensaje = "Meta marcada como completada." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _context.Metas.FindAsync(id);
        if (entity == null) return NotFound("Meta no encontrada.");
        _context.Metas.Remove(entity);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
