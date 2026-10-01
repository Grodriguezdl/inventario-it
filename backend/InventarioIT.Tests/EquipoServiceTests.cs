using InventarioIT.Api.Dtos;
using InventarioIT.Api.Exceptions;
using InventarioIT.Api.Models;
using InventarioIT.Api.Services;
using InventarioIT.Tests.Infraestructura;

namespace InventarioIT.Tests;

[Collection("Database")]
public class EquipoServiceTests
{
    private readonly DatabaseFixture _fixture;

    public EquipoServiceTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _fixture.LimpiarDatos();
    }

    private static EquipoRequest Request(string codigo = "LAP-010", int categoriaId = 1) => new()
    {
        CodigoInventario = codigo,
        Marca = "Dell",
        Modelo = "Latitude 5440",
        CategoriaId = categoriaId
    };

    [Fact]
    public async Task Crear_NormalizaElCodigoYQuedaDisponible()
    {
        using var db = _fixture.CreateContext();
        var service = new EquipoService(db);

        var resultado = await service.CrearAsync(Request("  lap-010 "), CancellationToken.None);

        Assert.Equal("LAP-010", resultado.CodigoInventario);
        Assert.Equal(EstadoEquipo.Disponible, resultado.Estado);
        Assert.Equal("Laptop", resultado.Categoria);
    }

    [Fact]
    public async Task Crear_ConCodigoDuplicado_LanzaConflict()
    {
        using var db = _fixture.CreateContext();
        await TestData.EquipoAsync(db, codigo: "LAP-001");
        var service = new EquipoService(db);

        // Mismo código en minúsculas: también debe detectarse como duplicado
        await Assert.ThrowsAsync<ConflictException>(
            () => service.CrearAsync(Request("lap-001"), CancellationToken.None));
    }

    [Fact]
    public async Task Crear_ConCategoriaInexistente_LanzaReglaNegocio()
    {
        using var db = _fixture.CreateContext();
        var service = new EquipoService(db);

        await Assert.ThrowsAsync<ReglaNegocioException>(
            () => service.CrearAsync(Request(categoriaId: 999), CancellationToken.None));
    }

    [Fact]
    public async Task Listar_BuscaSinDistinguirMayusculas()
    {
        using var db = _fixture.CreateContext();
        await TestData.EquipoAsync(db, codigo: "LAP-001", marca: "Dell");
        await TestData.EquipoAsync(db, codigo: "LAP-002", marca: "HP");
        var service = new EquipoService(db);

        var resultado = await service.ListarAsync(new EquipoFiltro { Busqueda = "dell" }, CancellationToken.None);

        Assert.Equal(1, resultado.TotalRegistros);
        Assert.Equal("LAP-001", resultado.Items[0].CodigoInventario);
    }

    [Fact]
    public async Task DarDeBaja_EquipoAsignado_LanzaConflict()
    {
        using var db = _fixture.CreateContext();
        var equipo = await TestData.EquipoAsync(db, estado: EstadoEquipo.Asignado);
        var service = new EquipoService(db);

        await Assert.ThrowsAsync<ConflictException>(
            () => service.DarDeBajaAsync(equipo.Id, CancellationToken.None));
    }

    [Theory]
    [InlineData(EstadoEquipo.Disponible, EstadoEquipo.EnReparacion)]
    [InlineData(EstadoEquipo.EnReparacion, EstadoEquipo.Disponible)]
    public async Task CambiarEstado_TransicionPermitida_ActualizaElEstado(
        EstadoEquipo inicial, EstadoEquipo nuevo)
    {
        using var db = _fixture.CreateContext();
        var equipo = await TestData.EquipoAsync(db, estado: inicial);
        var service = new EquipoService(db);

        var resultado = await service.CambiarEstadoAsync(equipo.Id, nuevo, CancellationToken.None);

        Assert.Equal(nuevo, resultado.Estado);
    }

    [Theory]
    [InlineData(EstadoEquipo.Disponible, EstadoEquipo.DeBaja)]
    [InlineData(EstadoEquipo.Disponible, EstadoEquipo.Asignado)]
    [InlineData(EstadoEquipo.Asignado, EstadoEquipo.Disponible)]
    [InlineData(EstadoEquipo.DeBaja, EstadoEquipo.Disponible)]
    public async Task CambiarEstado_TransicionNoPermitida_LanzaConflict(
        EstadoEquipo inicial, EstadoEquipo nuevo)
    {
        using var db = _fixture.CreateContext();
        var equipo = await TestData.EquipoAsync(db, estado: inicial);
        var service = new EquipoService(db);

        await Assert.ThrowsAsync<ConflictException>(
            () => service.CambiarEstadoAsync(equipo.Id, nuevo, CancellationToken.None));
    }
}