using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using InventarioIT.Api.Models;

namespace InventarioIT.Api.Dtos;

// Lo que la API devuelve
public record EquipoResponse(
    int Id,
    string CodigoInventario,
    string? NumeroSerie,
    string Marca,
    string Modelo,
    EstadoEquipo Estado,
    int CategoriaId,
    string Categoria,
    DateOnly? FechaAdquisicion,
    string? Notas,
    DateTime CreadoEn,
    DateTime ActualizadoEn)
{
    // Proyección reutilizable: EF la traduce a SQL y solo trae las columnas necesarias
    public static readonly Expression<Func<Equipo, EquipoResponse>> Proyeccion = e =>
        new EquipoResponse(
            e.Id,
            e.CodigoInventario,
            e.NumeroSerie,
            e.Marca,
            e.Modelo,
            e.Estado,
            e.CategoriaId,
            e.Categoria.Nombre,
            e.FechaAdquisicion,
            e.Notas,
            e.CreadoEn,
            e.ActualizadoEn);
}

// Lo que la API recibe al crear o editar
public class EquipoRequest
{
    [Required(ErrorMessage = "El código de inventario es obligatorio.")]
    [StringLength(30, ErrorMessage = "El código no puede superar 30 caracteres.")]
    public string CodigoInventario { get; set; } = string.Empty;

    [StringLength(100)]
    public string? NumeroSerie { get; set; }

    [Required(ErrorMessage = "La marca es obligatoria.")]
    [StringLength(80)]
    public string Marca { get; set; } = string.Empty;

    [Required(ErrorMessage = "El modelo es obligatorio.")]
    [StringLength(100)]
    public string Modelo { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Debe indicar una categoría válida.")]
    public int CategoriaId { get; set; }

    public DateOnly? FechaAdquisicion { get; set; }

    [StringLength(500)]
    public string? Notas { get; set; }
}

// Parámetros de búsqueda del listado
public class EquipoFiltro : FiltroPaginado
{
    public string? Busqueda { get; set; }
    public EstadoEquipo? Estado { get; set; }
    public int? CategoriaId { get; set; }
}