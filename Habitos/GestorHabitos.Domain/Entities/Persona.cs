namespace GestorHabitos.Domain.Entities;

public abstract class Persona
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public abstract string ObtenerInfo();
}
