namespace GestorHabitos.Domain.Entities;

public class Habito
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int Frecuencia { get; set; }
    public string? HoraRecordatorio { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public string? Color { get; set; }
    public string? Icono { get; set; }
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public int CategoriaHabitoId { get; set; }
    public CategoriaHabito CategoriaHabito { get; set; } = null!;
    public List<Registro> Registros { get; set; } = new();
    public List<Meta> Metas { get; set; } = new();
}
