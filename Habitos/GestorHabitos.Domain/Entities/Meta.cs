namespace GestorHabitos.Domain.Entities;

public class Meta
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int HabitoId { get; set; }
    public Habito Habito { get; set; } = null!;
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int ObjetivoDias { get; set; }
    public bool Completada { get; set; }
}
