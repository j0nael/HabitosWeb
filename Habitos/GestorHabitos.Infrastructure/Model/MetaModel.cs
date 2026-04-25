namespace GestorHabitos.Infrastructure.Model;

public class MetaModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int HabitoId { get; set; }
    public HabitoModel Habito { get; set; } = null!;
    public int UsuarioId { get; set; }
    public UsuarioModel Usuario { get; set; } = null!;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int ObjetivoDias { get; set; }
    public bool Completada { get; set; }

    public MetaModel() { }

    public MetaModel(string nombre, int habitoId, int usuarioId, DateTime fechaInicio, DateTime fechaFin, int objetivoDias)
    {
        Nombre = nombre;
        HabitoId = habitoId;
        UsuarioId = usuarioId;
        FechaInicio = fechaInicio;
        FechaFin = fechaFin;
        ObjetivoDias = objetivoDias;
    }
}
