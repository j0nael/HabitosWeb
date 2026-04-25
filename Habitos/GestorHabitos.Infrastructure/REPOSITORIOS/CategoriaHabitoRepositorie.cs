using GestorHabitos.Infrastructure.Core;
using GestorHabitos.Infrastructure.Data;
using GestorHabitos.Infrastructure.Interfaces;
using GestorHabitos.Infrastructure.Model;

namespace GestorHabitos.Infrastructure.REPOSITORIOS;

public class CategoriaHabitoRepositorie : Baserepositorie<CategoriaHabitoModel>, ICategoriaHabitoRepository
{
    public CategoriaHabitoRepositorie(AppDbContext context) : base(context) { }
}
