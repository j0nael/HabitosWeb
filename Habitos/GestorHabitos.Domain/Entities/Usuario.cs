namespace GestorHabitos.Domain.Entities;

public class Usuario : Persona
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    public bool Activo { get; set; } = true;
    public List<Habito> Habitos { get; set; } = new();
    public List<Meta> Metas { get; set; } = new();

    public override string ObtenerInfo() => $"{Nombre} - {Email}";
}
