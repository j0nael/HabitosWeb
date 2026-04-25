namespace GestorHabitos.Application.DTO;

public class HabitoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int Frecuencia { get; set; }
    public string? HoraRecordatorio { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public string? Color { get; set; }
    public string? Icono { get; set; }
    public int UsuarioId { get; set; }
    public int CategoriaHabitoId { get; set; }
}
