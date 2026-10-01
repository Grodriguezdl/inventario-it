using InventarioIT.Api.Dtos;

namespace InventarioIT.Api.Services;

public interface IAsignacionService
{
    Task<PaginaResultado<AsignacionResponse>> ListarAsync(AsignacionFiltro filtro, CancellationToken ct);
    Task<AsignacionResponse> ObtenerAsync(int id, CancellationToken ct);
    Task<AsignacionResponse> AsignarAsync(AsignarRequest request, int usuarioId, CancellationToken ct);
    Task<AsignacionResponse> DevolverAsync(int id, DevolucionRequest request, CancellationToken ct);
}