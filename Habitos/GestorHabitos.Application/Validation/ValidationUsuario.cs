using FluentValidation;
using GestorHabitos.Application.DTO;
using GestorHabitos.Infrastructure.Data;

namespace GestorHabitos.Application.Validation;

public class ValidationUsuario : AbstractValidator<UsuarioDto>
{
    private readonly AppDbContext _context;

    public ValidationUsuario(AppDbContext context)
    {
        _context = context;

        RuleFor(u => u.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("Máximo 100 caracteres.");

        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El email no tiene un formato válido.")
            .MaximumLength(200).WithMessage("Máximo 200 caracteres.")
            .Must((dto, email) =>
            {
                if (dto.Id == 0)
                    return !_context.Usuarios.Any(x => x.Email == email);
                return !_context.Usuarios.Any(x => x.Email == email && x.Id != dto.Id);
            })
            .WithMessage("El email ya está registrado.");
    }
}
