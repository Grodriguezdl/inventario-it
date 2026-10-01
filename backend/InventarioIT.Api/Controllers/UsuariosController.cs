using InventarioIT.Api.Dtos;
using InventarioIT.Api.Models;
using InventarioIT.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventarioIT.Api.Controllers;

[ApiController]
[Route("api/usuarios")]
[Authorize(Roles = nameof(RolUsuario.Admin))]
public class UsuariosController(IUsuarioService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UsuarioResponse>>> Listar(CancellationToken ct)
        => Ok(await service.ListarAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UsuarioResponse>> Obtener(int id, CancellationToken ct)
        => Ok(await service.ObtenerAsync(id, ct));

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UsuarioResponse>> Crear(CrearUsuarioRequest request, CancellationToken ct)
    {
        var usuario = await service.CrearAsync(request, ct);
        return CreatedAtAction(nameof(Obtener), new { id = usuario.Id }, usuario);
    }
    
}