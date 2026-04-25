namespace GestorHabitos.Application.DTO;

public class RegistroDto
{
    public int Id { get; set; }
    public int HabitoId { get; set; }
    public DateTime Fecha { get; set; }
    public bool Completado { get; set; }
    public string? Nota { get; set; }
}
