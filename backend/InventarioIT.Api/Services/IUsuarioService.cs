using InventarioIT.Api.Dtos;

namespace InventarioIT.Api.Services;

public interface IUsuarioService
{
    Task<IReadOnlyList<UsuarioResponse>> ListarAsync(CancellationToken ct);
    Task<UsuarioResponse> ObtenerAsync(int id, CancellationToken ct);
    Task<UsuarioResponse> CrearAsync(CrearUsuarioRequest request, CancellationToken ct);
}