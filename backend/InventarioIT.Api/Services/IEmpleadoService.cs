using InventarioIT.Api.Dtos;

namespace InventarioIT.Api.Services;

public interface IEmpleadoService
{
    Task<PaginaResultado<EmpleadoResponse>> ListarAsync(EmpleadoFiltro filtro, CancellationToken ct);
    Task<EmpleadoResponse> ObtenerAsync(int id, CancellationToken ct);
    Task<EmpleadoResponse> CrearAsync(EmpleadoRequest request, CancellationToken ct);
    Task<EmpleadoResponse> ActualizarAsync(int id, EmpleadoRequest request, CancellationToken ct);
    Task DesactivarAsync(int id, CancellationToken ct);
    Task ActivarAsync(int id, CancellationToken ct);
}