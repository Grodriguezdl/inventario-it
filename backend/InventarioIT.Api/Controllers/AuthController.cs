using InventarioIT.Api.Dtos;
using InventarioIT.Api.Extensions;
using InventarioIT.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventarioIT.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService, IUsuarioService usuarioService) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken ct)
        => Ok(await authService.LoginAsync(request, ct));

    // Devuelve los datos del usuario dueño del token
    [HttpGet("me")]
    public async Task<ActionResult<UsuarioResponse>> Me(CancellationToken ct)
        => Ok(await usuarioService.ObtenerAsync(User.ObtenerUsuarioId(), ct));
}