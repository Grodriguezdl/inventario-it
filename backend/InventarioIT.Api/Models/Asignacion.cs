namespace InventarioIT.Api.Models;

public class Asignacion
{
    public int Id { get; set; }
    public DateTime FechaAsignacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaDevolucion { get; set; }
    public string? Observaciones { get; set; }

    public int EquipoId { get; set; }
    public Equipo Equipo { get; set; } = null!;

    public int EmpleadoId { get; set; }
    public Empleado Empleado { get; set; } = null!;

    public int AsignadoPorId { get; set; }
    public Usuario AsignadoPor { get; set; } = null!;
}