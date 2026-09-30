namespace InventarioIT.Api.Dtos;

public record CategoriaResponse(int Id, string Nombre, string? Descripcion, int TotalEquipos);