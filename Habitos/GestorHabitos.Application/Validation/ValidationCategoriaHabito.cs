using FluentValidation;
using GestorHabitos.Application.DTO;
using GestorHabitos.Infrastructure.Data;

namespace GestorHabitos.Application.Validation;

public class ValidationCategoriaHabito : AbstractValidator<CategoriaHabitoDto>
{
    private readonly AppDbContext _context;

    public ValidationCategoriaHabito(AppDbContext context)
    {
        _context = context;

        RuleFor(c => c.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("Máximo 100 caracteres.")
            .Must((dto, nombre) =>
            {
                if (dto.Id == 0)
                    return !_context.CategoriasHabito.Any(x => x.Nombre == nombre);
                return !_context.CategoriasHabito.Any(x => x.Nombre == nombre && x.Id != dto.Id);
            })
            .WithMessage("La categoría ya existe.");
    }
}
