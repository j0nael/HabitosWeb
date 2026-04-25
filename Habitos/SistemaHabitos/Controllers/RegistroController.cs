using GestorHabitos.Application.DTO;
using GestorHabitos.Infrastructure.Data;
using GestorHabitos.Infrastructure.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SistemaHabitos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegistroController : ControllerBase
{
    private readonly AppDbContext _context;

    public RegistroController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _context.Registros
            .Include(r => r.Habito)
            .ToListAsync();

        var list = data.Select(r => new
        {
            r.Id,
            r.HabitoId,
            HabitoNombre = r.Habito.Nombre,
            r.Habito.UsuarioId,
            r.Fecha,
            r.Completado,
            r.Nota
        });
        return Ok(list);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var r = await _context.Registros
            .Include(r => r.Habito)
            .FirstOrDefaultAsync(r => r.Id == id);
        if (r == null) return NotFound("Registro no encontrado.");
        return Ok(r);
    }

    [HttpGet("habito/{habitoId}")]
    public async Task<IActionResult> GetByHabito(int habitoId)
    {
        var data = await _context.Registros
            .Where(r => r.HabitoId == habitoId)
            .OrderByDescending(r => r.Fecha)
            .ToListAsync();
        return Ok(data);
    }

    [HttpGet("usuario/{usuarioId}/fecha/{fecha}")]
    public async Task<IActionResult> GetByUsuarioFecha(int usuarioId, DateTime fecha)
    {
        var habitos = await _context.Habitos
            .Where(h => h.UsuarioId == usuarioId && h.Activo)
            .ToListAsync();

        var habitoIds = habitos.Select(h => h.Id).ToList();

        var registros = await _context.Registros
            .Where(r => habitoIds.Contains(r.HabitoId) && r.Fecha.Date == fecha.Date)
            .ToListAsync();

        var result = habitos.Select(h => new
        {
            h.Id,
            h.Nombre,
            h.Color,
            h.Icono,
            h.Frecuencia,
            Registro = registros.FirstOrDefault(r => r.HabitoId == h.Id) is { } reg
                ? new { reg.Id, reg.Completado, reg.Nota }
                : null
        });
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(RegistroDto dto)
    {
        var entity = new RegistroModel
        {
            HabitoId = dto.HabitoId,
            Fecha = dto.Fecha.Date,
            Completado = dto.Completado,
            Nota = dto.Nota
        };
        _context.Registros.Add(entity);
        await _context.SaveChangesAsync();
        dto.Id = entity.Id;
        return Ok(dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, RegistroDto dto)
    {
        var entity = await _context.Registros.FindAsync(id);
        if (entity == null) return NotFound("Registro no encontrado.");
        entity.HabitoId = dto.HabitoId;
        entity.Fecha = dto.Fecha.Date;
        entity.Completado = dto.Completado;
        entity.Nota = dto.Nota;
        await _context.SaveChangesAsync();
        return Ok(dto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _context.Registros.FindAsync(id);
        if (entity == null) return NotFound("Registro no encontrado.");
        _context.Registros.Remove(entity);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
