namespace InventarioIT.Api.Models;

public class Empleado
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    public string Puesto { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;

    public ICollection<Asignacion> Asignaciones { get; set; } = new List<Asignacion>();
}