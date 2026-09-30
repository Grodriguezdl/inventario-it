namespace InventarioIT.Api.Dtos;

public record PaginaResultado<T>(
    IReadOnlyList<T> Items,
    int Pagina,
    int TamanoPagina,
    int TotalRegistros)
{
    public int TotalPaginas => (int)Math.Ceiling(TotalRegistros / (double)TamanoPagina);
}