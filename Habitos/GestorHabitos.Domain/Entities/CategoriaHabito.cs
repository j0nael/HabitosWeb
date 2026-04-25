namespace GestorHabitos.Domain.Entities;

public class CategoriaHabito
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public List<Habito> Habitos { get; set; } = new();
}
