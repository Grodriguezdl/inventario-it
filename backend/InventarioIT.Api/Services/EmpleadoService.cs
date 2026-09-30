using InventarioIT.Api.Data;
using InventarioIT.Api.Dtos;
using InventarioIT.Api.Exceptions;
using InventarioIT.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace InventarioIT.Api.Services;

public class EmpleadoService(AppDbContext db) : IEmpleadoService
{
    public async Task<PaginaResultado<EmpleadoResponse>> ListarAsync(EmpleadoFiltro filtro, CancellationToken ct)
    {
        var query = db.Empleados.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filtro.Busqueda))
        {
            var patron = $"%{filtro.Busqueda.Trim()}%";
            query = query.Where(e =>
                EF.Functions.ILike(e.Nombre, patron) ||
                EF.Functions.ILike(e.Apellido, patron) ||
                EF.Functions.ILike(e.Email, patron));
        }

        if (!string.IsNullOrWhiteSpace(filtro.Departamento))
        {
            query = query.Where(e => EF.Functions.ILike(e.Departamento, filtro.Departamento.Trim()));
        }

        if (filtro.Activo.HasValue)
        {
            query = query.Where(e => e.Activo == filtro.Activo.Value);
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(e => e.Apellido).ThenBy(e => e.Nombre)
            .Skip(filtro.Omitir)
            .Take(filtro.TamanoPagina)
            .Select(EmpleadoResponse.Proyeccion)
            .ToListAsync(ct);

        return new PaginaResultado<EmpleadoResponse>(items, filtro.Pagina, filtro.TamanoPagina, total);
    }

    public async Task<EmpleadoResponse> ObtenerAsync(int id, CancellationToken ct)
    {
        return await db.Empleados
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(EmpleadoResponse.Proyeccion)
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException($"No existe un empleado con Id {id}.");
    }

    public async Task<EmpleadoResponse> CrearAsync(EmpleadoRequest request, CancellationToken ct)
    {
        var email = NormalizarEmail(request.Email);
        await ValidarEmailUnicoAsync(email, idActual: null, ct);

        var empleado = new Empleado
        {
            Nombre = request.Nombre.Trim(),
            Apellido = request.Apellido.Trim(),
            Email = email,
            Departamento = request.Departamento.Trim(),
            Puesto = request.Puesto.Trim()
        };

        db.Empleados.Add(empleado);
        await db.SaveChangesAsync(ct);

        return await ObtenerAsync(empleado.Id, ct);
    }

    public async Task<EmpleadoResponse> ActualizarAsync(int id, EmpleadoRequest request, CancellationToken ct)
    {
        var empleado = await BuscarAsync(id, ct);

        var email = NormalizarEmail(request.Email);
        await ValidarEmailUnicoAsync(email, idActual: id, ct);

        empleado.Nombre = request.Nombre.Trim();
        empleado.Apellido = request.Apellido.Trim();
        empleado.Email = email;
        empleado.Departamento = request.Departamento.Trim();
        empleado.Puesto = request.Puesto.Trim();

        await db.SaveChangesAsync(ct);

        return await ObtenerAsync(id, ct);
    }

    public async Task DesactivarAsync(int id, CancellationToken ct)
    {
        var empleado = await BuscarAsync(id, ct);

        var tieneEquipos = await db.Asignaciones
            .AnyAsync(a => a.EmpleadoId == id && a.FechaDevolucion == null, ct);

        if (tieneEquipos)
        {
            throw new ConflictException(
                "No se puede desactivar un empleado con equipos asignados. Registra primero sus devoluciones.");
        }

        empleado.Activo = false;
        await db.SaveChangesAsync(ct);
    }

    public async Task ActivarAsync(int id, CancellationToken ct)
    {
        var empleado = await BuscarAsync(id, ct);
        empleado.Activo = true;
        await db.SaveChangesAsync(ct);
    }

    private async Task<Empleado> BuscarAsync(int id, CancellationToken ct)
        => await db.Empleados.FindAsync([id], ct)
           ?? throw new NotFoundException($"No existe un empleado con Id {id}.");

    private static string NormalizarEmail(string email) => email.Trim().ToLowerInvariant();

    private async Task ValidarEmailUnicoAsync(string email, int? idActual, CancellationToken ct)
    {
        if (await db.Empleados.AnyAsync(e => e.Email == email && e.Id != idActual, ct))
        {
            throw new ConflictException($"Ya existe un empleado con el correo {email}.");
        }
    }
}