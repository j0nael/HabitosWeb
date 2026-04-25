using GestorHabitos.Application.DTO;
using GestorHabitos.Infrastructure.Data;
using GestorHabitos.Infrastructure.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SistemaHabitos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsuarioController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _context.Usuarios.ToListAsync();
        var list = data.Select(u => new UsuarioDto
        {
            Id = u.Id,
            Nombre = u.Nombre,
            Email = u.Email,
            PasswordHash = u.PasswordHash,
            FechaRegistro = u.FechaRegistro,
            Activo = u.Activo
        }).ToList();
        return Ok(list);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var u = await _context.Usuarios.FindAsync(id);
        if (u == null) return NotFound("Usuario no encontrado.");
        return Ok(new UsuarioDto
        {
            Id = u.Id,
            Nombre = u.Nombre,
            Email = u.Email,
            PasswordHash = u.PasswordHash,
            FechaRegistro = u.FechaRegistro,
            Activo = u.Activo
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(UsuarioDto dto)
    {
        var entity = new UsuarioModel
        {
            Nombre = dto.Nombre,
            Email = dto.Email,
            PasswordHash = dto.PasswordHash,
            FechaRegistro = DateTime.Now,
            Activo = true
        };
        _context.Usuarios.Add(entity);
        await _context.SaveChangesAsync();
        dto.Id = entity.Id;
        return Ok(dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UsuarioDto dto)
    {
        var entity = await _context.Usuarios.FindAsync(id);
        if (entity == null) return NotFound("Usuario no encontrado.");
        entity.Nombre = dto.Nombre;
        entity.Email = dto.Email;
        entity.PasswordHash = dto.PasswordHash;
        entity.Activo = dto.Activo;
        await _context.SaveChangesAsync();
        return Ok(dto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _context.Usuarios
            .Include(u => u.Habitos)
            .Include(u => u.Metas)
            .FirstOrDefaultAsync(u => u.Id == id);
        if (entity == null) return NotFound("Usuario no encontrado.");
        if (entity.Habitos.Any()) return BadRequest("No se puede eliminar, el usuario tiene hábitos registrados.");
        _context.Usuarios.Remove(entity);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
