using GestorHabitos.Infrastructure.Core;
using GestorHabitos.Infrastructure.Data;
using GestorHabitos.Infrastructure.Interfaces;
using GestorHabitos.Infrastructure.Model;

namespace GestorHabitos.Infrastructure.REPOSITORIOS;

public class UsuarioRepositorie : Baserepositorie<UsuarioModel>, IUsuarioRepository
{
    public UsuarioRepositorie(AppDbContext context) : base(context) { }
}
