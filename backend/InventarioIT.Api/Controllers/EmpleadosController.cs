using InventarioIT.Api.Dtos;
using InventarioIT.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventarioIT.Api.Controllers;

[ApiController]
[Route("api/empleados")]
public class EmpleadosController(IEmpleadoService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginaResultado<EmpleadoResponse>>> Listar(
        [FromQuery] EmpleadoFiltro filtro, CancellationToken ct)
        => Ok(await service.ListarAsync(filtro, ct));

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmpleadoResponse>> Obtener(int id, CancellationToken ct)
        => Ok(await service.ObtenerAsync(id, ct));

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<EmpleadoResponse>> Crear(EmpleadoRequest request, CancellationToken ct)
    {
        var empleado = await service.CrearAsync(request, ct);
        return CreatedAtAction(nameof(Obtener), new { id = empleado.Id }, empleado);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<EmpleadoResponse>> Actualizar(
        int id, EmpleadoRequest request, CancellationToken ct)
        => Ok(await service.ActualizarAsync(id, request, ct));

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Desactivar(int id, CancellationToken ct)
    {
        await service.DesactivarAsync(id, ct);
        return NoContent();
    }

    [HttpPost("{id:int}/activar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Activar(int id, CancellationToken ct)
    {
        await service.ActivarAsync(id, ct);
        return NoContent();
    }
}