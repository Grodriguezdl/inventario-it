using InventarioIT.Api.Data;
using InventarioIT.Api.Dtos;
using InventarioIT.Api.Exceptions;
using InventarioIT.Api.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace InventarioIT.Api.Services;

public class AsignacionService(AppDbContext db) : IAsignacionService
{
    public async Task<PaginaResultado<AsignacionResponse>> ListarAsync(AsignacionFiltro filtro, CancellationToken ct)
    {
        var query = db.Asignaciones.AsNoTracking();

        if (filtro.EquipoId.HasValue)
        {
            query = query.Where(a => a.EquipoId == filtro.EquipoId.Value);
        }

        if (filtro.EmpleadoId.HasValue)
        {
            query = query.Where(a => a.EmpleadoId == filtro.EmpleadoId.Value);
        }

        if (filtro.Activas.HasValue)
        {
            query = filtro.Activas.Value
                ? query.Where(a => a.FechaDevolucion == null)
                : query.Where(a => a.FechaDevolucion != null);
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(a => a.FechaAsignacion)
            .Skip(filtro.Omitir)
            .Take(filtro.TamanoPagina)
            .Select(AsignacionResponse.Proyeccion)
            .ToListAsync(ct);

        return new PaginaResultado<AsignacionResponse>(items, filtro.Pagina, filtro.TamanoPagina, total);
    }

    public async Task<AsignacionResponse> ObtenerAsync(int id, CancellationToken ct)
    {
        return await db.Asignaciones
            .AsNoTracking()
            .Where(a => a.Id == id)
            .Select(AsignacionResponse.Proyeccion)
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException($"No existe una asignación con Id {id}.");
    }

    public async Task<AsignacionResponse> AsignarAsync(AsignarRequest request, int usuarioId, CancellationToken ct)
    {
        var equipo = await db.Equipos.FindAsync([request.EquipoId], ct)
            ?? throw new ReglaNegocioException($"El equipo con Id {request.EquipoId} no existe.");

        if (equipo.Estado != EstadoEquipo.Disponible)
        {
            throw new ConflictException(equipo.Estado switch
            {
                EstadoEquipo.Asignado => $"El equipo {equipo.CodigoInventario} ya está asignado.",
                EstadoEquipo.EnReparacion => $"El equipo {equipo.CodigoInventario} está en reparación.",
                EstadoEquipo.DeBaja => $"El equipo {equipo.CodigoInventario} está dado de baja.",
                _ => $"El equipo {equipo.CodigoInventario} no está disponible."
            });
        }

        var empleado = await db.Empleados.FindAsync([request.EmpleadoId], ct)
            ?? throw new ReglaNegocioException($"El empleado con Id {request.EmpleadoId} no existe.");

        if (!empleado.Activo)
        {
            throw new ConflictException("No se pueden asignar equipos a un empleado inactivo.");
        }

        var asignacion = new Asignacion
        {
            EquipoId = equipo.Id,
            EmpleadoId = empleado.Id,
            AsignadoPorId = usuarioId,
            Observaciones = request.Observaciones?.Trim()
        };

        // Ambos cambios se guardan juntos en una sola transacción
        equipo.Estado = EstadoEquipo.Asignado;
        equipo.ActualizadoEn = DateTime.UtcNow;
        db.Asignaciones.Add(asignacion);

        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (
            ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            // El índice único detectó otra asignación activa creada al mismo tiempo
            throw new ConflictException($"El equipo {equipo.CodigoInventario} acaba de ser asignado por otro usuario.");
        }

        return await ObtenerAsync(asignacion.Id, ct);
    }

    public async Task<AsignacionResponse> DevolverAsync(int id, DevolucionRequest request, CancellationToken ct)
    {
        if (request.EstadoEquipo is not (EstadoEquipo.Disponible or EstadoEquipo.EnReparacion))
        {
            throw new ReglaNegocioException("Al devolver, el equipo solo puede quedar Disponible o EnReparacion.");
        }

        var asignacion = await db.Asignaciones
            .Include(a => a.Equipo)
            .FirstOrDefaultAsync(a => a.Id == id, ct)
            ?? throw new NotFoundException($"No existe una asignación con Id {id}.");

        if (asignacion.FechaDevolucion is not null)
        {
            throw new ConflictException("Esta asignación ya fue devuelta.");
        }

        asignacion.FechaDevolucion = DateTime.UtcNow;
        asignacion.ObservacionesDevolucion = request.Observaciones?.Trim();
        asignacion.Equipo.Estado = request.EstadoEquipo;
        asignacion.Equipo.ActualizadoEn = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        return await ObtenerAsync(id, ct);
    }
}