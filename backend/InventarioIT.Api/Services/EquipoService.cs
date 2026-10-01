using InventarioIT.Api.Data;
using InventarioIT.Api.Dtos;
using InventarioIT.Api.Exceptions;
using InventarioIT.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace InventarioIT.Api.Services;

public class EquipoService(AppDbContext db) : IEquipoService
{
    public async Task<PaginaResultado<EquipoResponse>> ListarAsync(EquipoFiltro filtro, CancellationToken ct)
    {
        var query = db.Equipos.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filtro.Busqueda))
        {
            var patron = $"%{filtro.Busqueda.Trim()}%";
            // ILike = búsqueda sin distinguir mayúsculas (propio de PostgreSQL)
            query = query.Where(e =>
                EF.Functions.ILike(e.CodigoInventario, patron) ||
                EF.Functions.ILike(e.Marca, patron) ||
                EF.Functions.ILike(e.Modelo, patron) ||
                (e.NumeroSerie != null && EF.Functions.ILike(e.NumeroSerie, patron)));
        }

        if (filtro.Estado.HasValue)
        {
            query = query.Where(e => e.Estado == filtro.Estado.Value);
        }

        if (filtro.CategoriaId.HasValue)
        {
            query = query.Where(e => e.CategoriaId == filtro.CategoriaId.Value);
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(e => e.CodigoInventario)
            .Skip(filtro.Omitir)
            .Take(filtro.TamanoPagina)
            .Select(EquipoResponse.Proyeccion)
            .ToListAsync(ct);

        return new PaginaResultado<EquipoResponse>(items, filtro.Pagina, filtro.TamanoPagina, total);
    }

    public async Task<EquipoResponse> ObtenerAsync(int id, CancellationToken ct)
    {
        return await db.Equipos
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(EquipoResponse.Proyeccion)
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException($"No existe un equipo con Id {id}.");
    }

    public async Task<EquipoResponse> CrearAsync(EquipoRequest request, CancellationToken ct)
    {
        var codigo = NormalizarCodigo(request.CodigoInventario);

        await ValidarCodigoUnicoAsync(codigo, idActual: null, ct);
        await ValidarCategoriaAsync(request.CategoriaId, ct);

        var equipo = new Equipo
        {
            CodigoInventario = codigo,
            NumeroSerie = request.NumeroSerie?.Trim(),
            Marca = request.Marca.Trim(),
            Modelo = request.Modelo.Trim(),
            CategoriaId = request.CategoriaId,
            FechaAdquisicion = request.FechaAdquisicion,
            Notas = request.Notas?.Trim()
        };

        db.Equipos.Add(equipo);
        await db.SaveChangesAsync(ct);

        return await ObtenerAsync(equipo.Id, ct);
    }

    public async Task<EquipoResponse> ActualizarAsync(int id, EquipoRequest request, CancellationToken ct)
    {
        var equipo = await db.Equipos.FindAsync([id], ct)
            ?? throw new NotFoundException($"No existe un equipo con Id {id}.");

        var codigo = NormalizarCodigo(request.CodigoInventario);

        await ValidarCodigoUnicoAsync(codigo, idActual: id, ct);
        await ValidarCategoriaAsync(request.CategoriaId, ct);

        equipo.CodigoInventario = codigo;
        equipo.NumeroSerie = request.NumeroSerie?.Trim();
        equipo.Marca = request.Marca.Trim();
        equipo.Modelo = request.Modelo.Trim();
        equipo.CategoriaId = request.CategoriaId;
        equipo.FechaAdquisicion = request.FechaAdquisicion;
        equipo.Notas = request.Notas?.Trim();
        equipo.ActualizadoEn = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        return await ObtenerAsync(id, ct);
    }

    public async Task DarDeBajaAsync(int id, CancellationToken ct)
    {
        var equipo = await db.Equipos.FindAsync([id], ct)
            ?? throw new NotFoundException($"No existe un equipo con Id {id}.");

        if (equipo.Estado == EstadoEquipo.Asignado)
        {
            throw new ConflictException(
                "No se puede dar de baja un equipo asignado. Registra primero su devolución.");
        }

        if (equipo.Estado == EstadoEquipo.DeBaja)
        {
            return; // Ya estaba de baja: no hay nada que hacer
        }

        // No se elimina el registro: se marca como de baja para conservar el historial
        equipo.Estado = EstadoEquipo.DeBaja;
        equipo.ActualizadoEn = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
    }
    public async Task<EquipoResponse> CambiarEstadoAsync(int id, EstadoEquipo nuevoEstado, CancellationToken ct)
    {
        var equipo = await db.Equipos.FindAsync([id], ct)
            ?? throw new NotFoundException($"No existe un equipo con Id {id}.");

        if (equipo.Estado == nuevoEstado)
        {
            return await ObtenerAsync(id, ct);
        }

        var permitido = (equipo.Estado, nuevoEstado) is
            (EstadoEquipo.Disponible, EstadoEquipo.EnReparacion) or
            (EstadoEquipo.EnReparacion, EstadoEquipo.Disponible);

        if (!permitido)
        {
            throw new ConflictException(
                $"No se puede cambiar el estado de {equipo.Estado} a {nuevoEstado}. " +
                "Usa los endpoints de asignaciones o de baja para esos cambios.");
        }

        equipo.Estado = nuevoEstado;
        equipo.ActualizadoEn = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        return await ObtenerAsync(id, ct);
    }
    private static string NormalizarCodigo(string codigo) => codigo.Trim().ToUpperInvariant();

    private async Task ValidarCodigoUnicoAsync(string codigo, int? idActual, CancellationToken ct)
    {
        var existe = await db.Equipos.AnyAsync(
            e => e.CodigoInventario == codigo && e.Id != idActual, ct);

        if (existe)
        {
            throw new ConflictException($"Ya existe un equipo con el código {codigo}.");
        }
    }

    private async Task ValidarCategoriaAsync(int categoriaId, CancellationToken ct)
    {
        if (!await db.Categorias.AnyAsync(c => c.Id == categoriaId, ct))
        {
            throw new ReglaNegocioException($"La categoría con Id {categoriaId} no existe.");
        }
    }
}