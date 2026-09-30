using InventarioIT.Api.Dtos;

namespace InventarioIT.Api.Services;

public interface ICategoriaService
{
    Task<IReadOnlyList<CategoriaResponse>> ListarAsync(CancellationToken ct);
}