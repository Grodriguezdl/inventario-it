using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using InventarioIT.Api.Models;

namespace InventarioIT.Api.Dtos;

public record AsignacionResponse(
    int Id,
    int EquipoId,
    string CodigoInventario,
    string Equipo,
    int EmpleadoId,
    string Empleado,
    string Departamento,
    string AsignadoPor,
    DateTime FechaAsignacion,
    DateTime? FechaDevolucion,
    string? Observaciones,
    string? ObservacionesDevolucion,
    bool Activa)
{
    public static readonly Expression<Func<Asignacion, AsignacionResponse>> Proyeccion = a =>
        new AsignacionResponse(
            a.Id,
            a.EquipoId,
            a.Equipo.CodigoInventario,
            a.Equipo.Marca + " " + a.Equipo.Modelo,
            a.EmpleadoId,
            a.Empleado.Nombre + " " + a.Empleado.Apellido,
            a.Empleado.Departamento,
            a.AsignadoPor.Nombre,
            a.FechaAsignacion,
            a.FechaDevolucion,
            a.Observaciones,
            a.ObservacionesDevolucion,
            a.FechaDevolucion == null);
}

public class AsignarRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Debe indicar un equipo válido.")]
    public int EquipoId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe indicar un empleado válido.")]
    public int EmpleadoId { get; set; }

    [StringLength(500)]
    public string? Observaciones { get; set; }
}

public class DevolucionRequest
{
    // Estado en que queda el equipo: Disponible o EnReparacion
    public EstadoEquipo EstadoEquipo { get; set; } = EstadoEquipo.Disponible;

    [StringLength(500)]
    public string? Observaciones { get; set; }
}

public class AsignacionFiltro : FiltroPaginado
{
    public int? EquipoId { get; set; }
    public int? EmpleadoId { get; set; }
    public bool? Activas { get; set; }
}