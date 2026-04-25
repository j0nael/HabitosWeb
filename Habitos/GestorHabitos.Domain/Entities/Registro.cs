namespace GestorHabitos.Domain.Entities;

public class Registro
{
    public int Id { get; set; }
    public int HabitoId { get; set; }
    public Habito Habito { get; set; } = null!;
    public DateTime Fecha { get; set; } = DateTime.Today;
    public bool Completado { get; set; }
    public string? Nota { get; set; }
}
