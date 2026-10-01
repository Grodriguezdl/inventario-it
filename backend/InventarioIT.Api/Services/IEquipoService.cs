using InventarioIT.Api.Dtos;
using InventarioIT.Api.Models;
namespace InventarioIT.Api.Services;

public interface IEquipoService
{
    Task<PaginaResultado<EquipoResponse>> ListarAsync(EquipoFiltro filtro, CancellationToken ct);
    Task<EquipoResponse> ObtenerAsync(int id, CancellationToken ct);
    Task<EquipoResponse> CrearAsync(EquipoRequest request, CancellationToken ct);
    Task<EquipoResponse> ActualizarAsync(int id, EquipoRequest request, CancellationToken ct);
    Task DarDeBajaAsync(int id, CancellationToken ct);
    Task<EquipoResponse> CambiarEstadoAsync(int id, EstadoEquipo nuevoEstado, CancellationToken ct);

}