using InventarioIT.Api.Data;
using InventarioIT.Api.Dtos;
using InventarioIT.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace InventarioIT.Api.Services;

public class DashboardService(AppDbContext db) : IDashboardService
{
    public async Task<DashboardResponse> ObtenerAsync(CancellationToken ct)
    {
        // Equipos activos: todos excepto los dados de baja
        var totalEquipos = await db.Equipos
            .CountAsync(e => e.Estado != EstadoEquipo.DeBaja, ct);

        var empleadosActivos = await db.Empleados.CountAsync(e => e.Activo, ct);

        var asignacionesActivas = await db.Asignaciones
            .CountAsync(a => a.FechaDevolucion == null, ct);

        var conteos = await db.Equipos
            .GroupBy(e => e.Estado)
            .Select(g => new { Estado = g.Key, Total = g.Count() })
            .ToListAsync(ct);

        // Incluimos todos los estados, aunque tengan 0 equipos (útil para las gráficas)
        var porEstado = Enum.GetValues<EstadoEquipo>()
            .Select(estado => new ConteoPorEstado(
                estado,
                conteos.FirstOrDefault(c => c.Estado == estado)?.Total ?? 0))
            .ToList();

        var porCategoria = await db.Categorias
            .OrderBy(c => c.Nombre)
            .Select(c => new ConteoPorCategoria(
                c.Nombre,
                c.Equipos.Count(e => e.Estado != EstadoEquipo.DeBaja)))
            .ToListAsync(ct);

        var ultimas = await db.Asignaciones
            .AsNoTracking()
            .OrderByDescending(a => a.FechaAsignacion)
            .Take(5)
            .Select(AsignacionResponse.Proyeccion)
            .ToListAsync(ct);

        return new DashboardResponse(
            totalEquipos, empleadosActivos, asignacionesActivas, porEstado, porCategoria, ultimas);
    }
}