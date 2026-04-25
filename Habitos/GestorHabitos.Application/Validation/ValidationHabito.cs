using FluentValidation;
using GestorHabitos.Application.DTO;
using GestorHabitos.Infrastructure.Data;

namespace GestorHabitos.Application.Validation;

public class ValidationHabito : AbstractValidator<HabitoDto>
{
    private readonly AppDbContext _context;

    public ValidationHabito(AppDbContext context)
    {
        _context = context;

        RuleFor(h => h.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(200).WithMessage("Máximo 200 caracteres.");

        RuleFor(h => h.Frecuencia)
            .InclusiveBetween(0, 2).WithMessage("Frecuencia inválida. Use 0=Diario, 1=Semanal, 2=Mensual.");

        RuleFor(h => h.UsuarioId)
            .GreaterThan(0).WithMessage("Debe seleccionar un usuario válido.")
            .Must(id => _context.Usuarios.Any(u => u.Id == id))
            .WithMessage("El usuario no existe.");

        RuleFor(h => h.CategoriaHabitoId)
            .GreaterThan(0).WithMessage("Debe seleccionar una categoría válida.")
            .Must(id => _context.CategoriasHabito.Any(c => c.Id == id))
            .WithMessage("La categoría no existe.");
    }
}
