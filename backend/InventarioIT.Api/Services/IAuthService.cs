using InventarioIT.Api.Dtos;

namespace InventarioIT.Api.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct);
}