using System.ComponentModel.DataAnnotations;

namespace InventarioIT.Api.Dtos;

public record PaginaResultado<T>(
    IReadOnlyList<T> Items,
    int Pagina,
    int TamanoPagina,
    int TotalRegistros)
{
    public int TotalPaginas => (int)Math.Ceiling(TotalRegistros / (double)TamanoPagina);
}

// Base para cualquier listado paginado
public abstract class FiltroPaginado
{
    [Range(1, int.MaxValue)]
    public int Pagina { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "El tamaño de página debe estar entre 1 y 100.")]
    public int TamanoPagina { get; set; } = 10;

    public int Omitir => (Pagina - 1) * TamanoPagina;
}