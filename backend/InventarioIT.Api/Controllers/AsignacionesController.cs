using InventarioIT.Api.Dtos;
using InventarioIT.Api.Extensions;
using InventarioIT.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventarioIT.Api.Controllers;

[ApiController]
[Route("api/asignaciones")]
public class AsignacionesController(IAsignacionService service) : ControllerBase
{
    // Sirve también como historial: filtra por equipoId o empleadoId
    [HttpGet]
    public async Task<ActionResult<PaginaResultado<AsignacionResponse>>> Listar(
        [FromQuery] AsignacionFiltro filtro, CancellationToken ct)
        => Ok(await service.ListarAsync(filtro, ct));

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AsignacionResponse>> Obtener(int id, CancellationToken ct)
        => Ok(await service.ObtenerAsync(id, ct));

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AsignacionResponse>> Asignar(AsignarRequest request, CancellationToken ct)
    {
        // El usuario que asigna se obtiene del token, no del cuerpo de la petición
        var asignacion = await service.AsignarAsync(request, User.ObtenerUsuarioId(), ct);
        return CreatedAtAction(nameof(Obtener), new { id = asignacion.Id }, asignacion);
    }

    [HttpPost("{id:int}/devolucion")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AsignacionResponse>> Devolver(
        int id, DevolucionRequest request, CancellationToken ct)
        => Ok(await service.DevolverAsync(id, request, ct));
}