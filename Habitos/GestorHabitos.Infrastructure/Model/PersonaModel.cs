namespace GestorHabitos.Infrastructure.Model;

public abstract class PersonaModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public abstract string ObtenerInfo();
}
