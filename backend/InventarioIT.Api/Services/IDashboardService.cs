using InventarioIT.Api.Dtos;

namespace InventarioIT.Api.Services;

public interface IDashboardService
{
    Task<DashboardResponse> ObtenerAsync(CancellationToken ct);
}