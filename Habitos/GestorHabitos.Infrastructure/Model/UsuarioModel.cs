namespace GestorHabitos.Infrastructure.Model;

public class UsuarioModel : PersonaModel
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    public bool Activo { get; set; } = true;
    public List<HabitoModel> Habitos { get; set; } = new();
    public List<MetaModel> Metas { get; set; } = new();

    public UsuarioModel() { }

    public UsuarioModel(string nombre, string email)
    {
        Nombre = nombre;
        Email = email;
    }

    public override string ObtenerInfo() => $"{Nombre} - {Email}";
}
