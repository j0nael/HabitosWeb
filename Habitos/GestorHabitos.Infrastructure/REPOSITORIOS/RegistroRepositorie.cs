using GestorHabitos.Infrastructure.Core;
using GestorHabitos.Infrastructure.Data;
using GestorHabitos.Infrastructure.Interfaces;
using GestorHabitos.Infrastructure.Model;

namespace GestorHabitos.Infrastructure.REPOSITORIOS;

public class RegistroRepositorie : Baserepositorie<RegistroModel>, IRegistroRepository
{
    public RegistroRepositorie(AppDbContext context) : base(context) { }
}
