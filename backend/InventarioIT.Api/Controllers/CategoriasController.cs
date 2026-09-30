using InventarioIT.Api.Dtos;
using InventarioIT.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventarioIT.Api.Controllers;

[ApiController]
[Route("api/categorias")]
public class CategoriasController(ICategoriaService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoriaResponse>>> Listar(CancellationToken ct)
        => Ok(await service.ListarAsync(ct));
}