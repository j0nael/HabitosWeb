using FluentValidation;
using GestorHabitos.Application.DTO;
using GestorHabitos.Infrastructure.Data;

namespace GestorHabitos.Application.Validation;

public class ValidationRegistro : AbstractValidator<RegistroDto>
{
    private readonly AppDbContext _context;

    public ValidationRegistro(AppDbContext context)
    {
        _context = context;

        RuleFor(r => r.HabitoId)
            .GreaterThan(0).WithMessage("Debe seleccionar un hábito válido.")
            .Must(id => _context.Habitos.Any(h => h.Id == id))
            .WithMessage("El hábito no existe.");

        RuleFor(r => r.Fecha)
            .NotEmpty().WithMessage("La fecha es obligatoria.")
            .LessThanOrEqualTo(DateTime.Today).WithMessage("No se puede registrar una fecha futura.");

        RuleFor(r => r)
            .Must(dto =>
            {
                if (dto.Id == 0)
                    return !_context.Registros.Any(x => x.HabitoId == dto.HabitoId && x.Fecha.Date == dto.Fecha.Date);
                return !_context.Registros.Any(x => x.HabitoId == dto.HabitoId && x.Fecha.Date == dto.Fecha.Date && x.Id != dto.Id);
            })
            .WithMessage("Ya existe un registro para este hábito en esa fecha.");
    }
}
