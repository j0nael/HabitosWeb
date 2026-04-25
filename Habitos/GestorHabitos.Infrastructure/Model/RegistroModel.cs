namespace GestorHabitos.Infrastructure.Model;

public class RegistroModel
{
    public int Id { get; set; }
    public int HabitoId { get; set; }
    public HabitoModel Habito { get; set; } = null!;
    public DateTime Fecha { get; set; } = DateTime.Today;
    public bool Completado { get; set; }
    public string? Nota { get; set; }

    public RegistroModel() { }

    public RegistroModel(int habitoId, DateTime fecha, bool completado)
    {
        HabitoId = habitoId;
        Fecha = fecha;
        Completado = completado;
    }
}
