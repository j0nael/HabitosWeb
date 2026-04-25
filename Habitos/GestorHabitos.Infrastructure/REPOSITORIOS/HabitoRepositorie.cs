using GestorHabitos.Infrastructure.Core;
using GestorHabitos.Infrastructure.Data;
using GestorHabitos.Infrastructure.Interfaces;
using GestorHabitos.Infrastructure.Model;

namespace GestorHabitos.Infrastructure.REPOSITORIOS;

public class HabitoRepositorie : Baserepositorie<HabitoModel>, IHabitoRepository
{
    public HabitoRepositorie(AppDbContext context) : base(context) { }
}
