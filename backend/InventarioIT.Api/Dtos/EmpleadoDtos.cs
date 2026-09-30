using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using InventarioIT.Api.Models;

namespace InventarioIT.Api.Dtos;

public record EmpleadoResponse(
    int Id,
    string Nombre,
    string Apellido,
    string Email,
    string Departamento,
    string Puesto,
    bool Activo,
    int EquiposAsignados)
{
    public static readonly Expression<Func<Empleado, EmpleadoResponse>> Proyeccion = e =>
        new EmpleadoResponse(
            e.Id,
            e.Nombre,
            e.Apellido,
            e.Email,
            e.Departamento,
            e.Puesto,
            e.Activo,
            // Solo cuentan las asignaciones sin devolución
            e.Asignaciones.Count(a => a.FechaDevolucion == null));
}

public class EmpleadoRequest
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(100)]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El departamento es obligatorio.")]
    [StringLength(100)]
    public string Departamento { get; set; } = string.Empty;

    [Required(ErrorMessage = "El puesto es obligatorio.")]
    [StringLength(100)]
    public string Puesto { get; set; } = string.Empty;
}

public class EmpleadoFiltro : FiltroPaginado
{
    public string? Busqueda { get; set; }
    public string? Departamento { get; set; }
    public bool? Activo { get; set; }
}