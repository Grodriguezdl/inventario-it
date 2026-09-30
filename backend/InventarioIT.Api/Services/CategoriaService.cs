using InventarioIT.Api.Data;
using InventarioIT.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace InventarioIT.Api.Services;

public class CategoriaService(AppDbContext db) : ICategoriaService
{
    public async Task<IReadOnlyList<CategoriaResponse>> ListarAsync(CancellationToken ct)
    {
        return await db.Categorias
            .AsNoTracking()
            .OrderBy(c => c.Nombre)
            .Select(c => new CategoriaResponse(c.Id, c.Nombre, c.Descripcion, c.Equipos.Count))
            .ToListAsync(ct);
    }
}