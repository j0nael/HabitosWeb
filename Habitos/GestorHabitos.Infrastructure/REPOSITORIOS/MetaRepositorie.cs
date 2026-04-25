using GestorHabitos.Infrastructure.Core;
using GestorHabitos.Infrastructure.Data;
using GestorHabitos.Infrastructure.Interfaces;
using GestorHabitos.Infrastructure.Model;

namespace GestorHabitos.Infrastructure.REPOSITORIOS;

public class MetaRepositorie : Baserepositorie<MetaModel>, IMetaRepository
{
    public MetaRepositorie(AppDbContext context) : base(context) { }
}
