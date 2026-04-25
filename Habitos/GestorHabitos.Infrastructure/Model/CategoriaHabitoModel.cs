namespace GestorHabitos.Infrastructure.Model;

public class CategoriaHabitoModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public List<HabitoModel> Habitos { get; set; } = new();

    public CategoriaHabitoModel() { }

    public CategoriaHabitoModel(int id, string nombre)
    {
        Id = id;
        Nombre = nombre;
    }
}
