namespace InventarioIT.Api.Models;

public class Categoria
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }

    public ICollection<Equipo> Equipos { get; set; } = new List<Equipo>();
}