using InventarioIT.Api.Dtos;
using InventarioIT.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventarioIT.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController(IDashboardService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<DashboardResponse>> Obtener(CancellationToken ct)
        => Ok(await service.ObtenerAsync(ct));
}