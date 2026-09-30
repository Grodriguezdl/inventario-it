using InventarioIT.Api.Dtos;
using InventarioIT.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventarioIT.Api.Controllers;

[ApiController]
[Route("api/equipos")]
public class EquiposController(IEquipoService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginaResultado<EquipoResponse>>> Listar(
        [FromQuery] EquipoFiltro filtro, CancellationToken ct)
        => Ok(await service.ListarAsync(filtro, ct));

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EquipoResponse>> Obtener(int id, CancellationToken ct)
        => Ok(await service.ObtenerAsync(id, ct));

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<EquipoResponse>> Crear(EquipoRequest request, CancellationToken ct)
    {
        var equipo = await service.CrearAsync(request, ct);
        return CreatedAtAction(nameof(Obtener), new { id = equipo.Id }, equipo);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<EquipoResponse>> Actualizar(
        int id, EquipoRequest request, CancellationToken ct)
        => Ok(await service.ActualizarAsync(id, request, ct));

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DarDeBaja(int id, CancellationToken ct)
    {
        await service.DarDeBajaAsync(id, ct);
        return NoContent();
    }
}