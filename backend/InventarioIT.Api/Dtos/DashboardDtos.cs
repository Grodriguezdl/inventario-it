using InventarioIT.Api.Models;

namespace InventarioIT.Api.Dtos;

public record ConteoPorEstado(EstadoEquipo Estado, int Total);

public record ConteoPorCategoria(string Categoria, int Total);

public record DashboardResponse(
    int TotalEquipos,
    int EmpleadosActivos,
    int AsignacionesActivas,
    IReadOnlyList<ConteoPorEstado> EquiposPorEstado,
    IReadOnlyList<ConteoPorCategoria> EquiposPorCategoria,
    IReadOnlyList<AsignacionResponse> UltimasAsignaciones);