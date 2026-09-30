namespace InventarioIT.Api.Models;

public class Equipo
{
    public int Id { get; set; }
    public string CodigoInventario { get; set; } = string.Empty;
    public string? NumeroSerie { get; set; }
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public EstadoEquipo Estado { get; set; } = EstadoEquipo.Disponible;
    public DateOnly? FechaAdquisicion { get; set; }
    public string? Notas { get; set; }
    public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
    public DateTime ActualizadoEn { get; set; } = DateTime.UtcNow;

    public int CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;

    public ICollection<Asignacion> Asignaciones { get; set; } = new List<Asignacion>();
}