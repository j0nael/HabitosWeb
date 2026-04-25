namespace GestorHabitos.Infrastructure.Model;

public class HabitoModel
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
    public UsuarioModel Usuario { get; set; } = null!;
    public int CategoriaHabitoId { get; set; }
    public CategoriaHabitoModel CategoriaHabito { get; set; } = null!;
    public List<RegistroModel> Registros { get; set; } = new();

    public HabitoModel() { }

    public HabitoModel(string nombre, int frecuencia, int usuarioId, int categoriaHabitoId)
    {
        Nombre = nombre;
        Frecuencia = frecuencia;
        UsuarioId = usuarioId;
        CategoriaHabitoId = categoriaHabitoId;
    }
}
