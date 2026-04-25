using GestorHabitos.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SistemaHabitos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstadisticasController : ControllerBase
{
    private readonly AppDbContext _context;

    public EstadisticasController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("usuario/{usuarioId}")]
    public async Task<IActionResult> GetUsuarioStats(int usuarioId)
    {
        var usuario = await _context.Usuarios.FindAsync(usuarioId);
        if (usuario == null) return NotFound("Usuario no encontrado.");

        var habitos = await _context.Habitos
            .Where(h => h.UsuarioId == usuarioId)
            .ToListAsync();

        var habitoIds = habitos.Select(h => h.Id).ToList();
        var hoy = DateTime.Today;
        var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);

        var registrosMes = await _context.Registros
            .Where(r => habitoIds.Contains(r.HabitoId) && r.Fecha >= inicioMes)
            .ToListAsync();

        var registrosHoy = registrosMes.Count(r => r.Fecha.Date == hoy && r.Completado);
        var completadosMes = registrosMes.Count(r => r.Completado);
        var totalMes = registrosMes.Count;
        var tasaMes = totalMes > 0 ? Math.Round((double)completadosMes / totalMes * 100, 1) : 0;

        var metas = await _context.Metas.Where(m => m.UsuarioId == usuarioId).ToListAsync();
        var metasCompletadas = metas.Count(m => m.Completada);

        var rachaActual = await CalcularRachaUsuario(habitoIds, hoy);

        return Ok(new
        {
            TotalHabitos = habitos.Count,
            HabitosActivos = habitos.Count(h => h.Activo),
            RegistrosHoy = registrosHoy,
            TasaCumplimientoMes = tasaMes,
            RachaActual = rachaActual,
            MetasCompletadas = metasCompletadas,
            MetasTotales = metas.Count
        });
    }

    [HttpGet("habito/{habitoId}")]
    public async Task<IActionResult> GetHabitoStats(int habitoId)
    {
        var habito = await _context.Habitos.FindAsync(habitoId);
        if (habito == null) return NotFound("Hábito no encontrado.");

        var registros = await _context.Registros
            .Where(r => r.HabitoId == habitoId)
            .OrderByDescending(r => r.Fecha)
            .ToListAsync();

        var total = registros.Count;
        var completados = registros.Count(r => r.Completado);
        var tasa = total > 0 ? Math.Round((double)completados / total * 100, 1) : 0;

        var rachaActual = CalcularRachaHabito(registros.Select(r => r.Fecha.Date).ToList());
        var rachaMasLarga = CalcularRachaMasLarga(registros.Select(r => r.Fecha.Date).ToList());

        var porMes = registros
            .GroupBy(r => new { r.Fecha.Year, r.Fecha.Month })
            .Select(g => new
            {
                Mes = $"{g.Key.Year}-{g.Key.Month:D2}",
                Total = g.Count(),
                Completados = g.Count(r => r.Completado)
            })
            .OrderBy(x => x.Mes)
            .ToList();

        return Ok(new
        {
            TotalRegistros = total,
            Completados = completados,
            NoCompletados = total - completados,
            TasaCumplimiento = tasa,
            RachaActual = rachaActual,
            RachaMasLarga = rachaMasLarga,
            HistoricoPorMes = porMes
        });
    }

    private async Task<int> CalcularRachaUsuario(List<int> habitoIds, DateTime hoy)
    {
        var fechasCompletadas = await _context.Registros
            .Where(r => habitoIds.Contains(r.HabitoId) && r.Completado)
            .Select(r => r.Fecha.Date)
            .Distinct()
            .OrderByDescending(f => f)
            .ToListAsync();

        int racha = 0;
        var fechaCheck = hoy;
        foreach (var fecha in fechasCompletadas)
        {
            if (fecha == fechaCheck)
            {
                racha++;
                fechaCheck = fechaCheck.AddDays(-1);
            }
            else break;
        }
        return racha;
    }

    private static int CalcularRachaHabito(List<DateTime> fechasCompletadas)
    {
        if (!fechasCompletadas.Any()) return 0;
        var ordenadas = fechasCompletadas.Distinct().OrderByDescending(f => f).ToList();
        int racha = 0;
        var hoy = DateTime.Today;
        var fechaCheck = hoy;
        foreach (var fecha in ordenadas)
        {
            if (fecha == fechaCheck)
            {
                racha++;
                fechaCheck = fechaCheck.AddDays(-1);
            }
            else break;
        }
        return racha;
    }

    private static int CalcularRachaMasLarga(List<DateTime> fechas)
    {
        if (!fechas.Any()) return 0;
        var ordenadas = fechas.Distinct().OrderBy(f => f).ToList();
        int maxRacha = 1, racha = 1;
        for (int i = 1; i < ordenadas.Count; i++)
        {
            if ((ordenadas[i] - ordenadas[i - 1]).Days == 1)
            {
                racha++;
                maxRacha = Math.Max(maxRacha, racha);
            }
            else racha = 1;
        }
        return maxRacha;
    }
}
