using FluentValidation;
using GestorHabitos.Application.DTO;
using GestorHabitos.Infrastructure.Data;

namespace GestorHabitos.Application.Validation;

public class ValidationMeta : AbstractValidator<MetaDto>
{
    private readonly AppDbContext _context;

    public ValidationMeta(AppDbContext context)
    {
        _context = context;

        RuleFor(m => m.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(200).WithMessage("Máximo 200 caracteres.");

        RuleFor(m => m.HabitoId)
            .GreaterThan(0).WithMessage("Debe seleccionar un hábito válido.")
            .Must(id => _context.Habitos.Any(h => h.Id == id))
            .WithMessage("El hábito no existe.");

        RuleFor(m => m.UsuarioId)
            .GreaterThan(0).WithMessage("Debe seleccionar un usuario válido.")
            .Must(id => _context.Usuarios.Any(u => u.Id == id))
            .WithMessage("El usuario no existe.");

        RuleFor(m => m.FechaInicio)
            .NotEmpty().WithMessage("La fecha de inicio es obligatoria.");

        RuleFor(m => m.FechaFin)
            .NotEmpty().WithMessage("La fecha de fin es obligatoria.")
            .GreaterThan(m => m.FechaInicio).WithMessage("La fecha de fin debe ser mayor que la de inicio.");

        RuleFor(m => m.ObjetivoDias)
            .GreaterThan(0).WithMessage("El objetivo debe ser mayor a 0.")
            .Must((dto, dias) => dias <= (dto.FechaFin - dto.FechaInicio).Days + 1)
            .WithMessage("El objetivo no puede superar la duración de la meta.");
    }
}
