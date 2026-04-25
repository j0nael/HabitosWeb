using GestorHabitos.Application.DTO;
using GestorHabitos.Infrastructure.Data;
using GestorHabitos.Infrastructure.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SistemaHabitos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriaHabitoController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriaHabitoController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _context.CategoriasHabito.ToListAsync();
        var list = data.Select(c => new CategoriaHabitoDto { Id = c.Id, Nombre = c.Nombre }).ToList();
        return Ok(list);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var c = await _context.CategoriasHabito.FindAsync(id);
        if (c == null) return NotFound("Categoría no encontrada.");
        return Ok(new CategoriaHabitoDto { Id = c.Id, Nombre = c.Nombre });
    }

    [HttpPost]
    public async Task<IActionResult> Create(CategoriaHabitoDto dto)
    {
        var entity = new CategoriaHabitoModel { Nombre = dto.Nombre };
        _context.CategoriasHabito.Add(entity);
        await _context.SaveChangesAsync();
        dto.Id = entity.Id;
        return Ok(dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CategoriaHabitoDto dto)
    {
        var entity = await _context.CategoriasHabito.FindAsync(id);
        if (entity == null) return NotFound("Categoría no encontrada.");
        entity.Nombre = dto.Nombre;
        await _context.SaveChangesAsync();
        return Ok(dto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _context.CategoriasHabito
            .Include(c => c.Habitos)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (entity == null) return NotFound("Categoría no encontrada.");
        if (entity.Habitos.Any()) return BadRequest("No se puede eliminar, tiene hábitos asociados.");
        _context.CategoriasHabito.Remove(entity);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
