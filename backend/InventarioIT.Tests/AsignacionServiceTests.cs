using InventarioIT.Api.Dtos;
using InventarioIT.Api.Exceptions;
using InventarioIT.Api.Models;
using InventarioIT.Api.Services;
using InventarioIT.Tests.Infraestructura;
using Microsoft.EntityFrameworkCore;

namespace InventarioIT.Tests;

[Collection("Database")]
public class AsignacionServiceTests
{
    private readonly DatabaseFixture _fixture;

    public AsignacionServiceTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _fixture.LimpiarDatos();
    }

    [Fact]
    public async Task Asignar_EquipoDisponible_QuedaAsignadoYRegistraQuienAsigno()
    {
        using var db = _fixture.CreateContext();
        var usuario = await TestData.UsuarioAsync(db);
        var empleado = await TestData.EmpleadoAsync(db);
        var equipo = await TestData.EquipoAsync(db);
        var service = new AsignacionService(db);

        var resultado = await service.AsignarAsync(
            new AsignarRequest { EquipoId = equipo.Id, EmpleadoId = empleado.Id },
            usuario.Id,
            CancellationToken.None);

        Assert.True(resultado.Activa);
        Assert.Equal(usuario.Nombre, resultado.AsignadoPor);

        // Verificamos con un contexto nuevo, leyendo lo que realmente quedó en la base
        using var verificacion = _fixture.CreateContext();
        var equipoGuardado = await verificacion.Equipos.FindAsync(equipo.Id);
        Assert.Equal(EstadoEquipo.Asignado, equipoGuardado!.Estado);
    }

    [Fact]
    public async Task Asignar_EquipoYaAsignado_LanzaConflict()
    {
        using var db = _fixture.CreateContext();
        var usuario = await TestData.UsuarioAsync(db);
        var empleado = await TestData.EmpleadoAsync(db);
        var equipo = await TestData.EquipoAsync(db);
        var service = new AsignacionService(db);
        var request = new AsignarRequest { EquipoId = equipo.Id, EmpleadoId = empleado.Id };

        await service.AsignarAsync(request, usuario.Id, CancellationToken.None);

        await Assert.ThrowsAsync<ConflictException>(
            () => service.AsignarAsync(request, usuario.Id, CancellationToken.None));
    }

    [Fact]
    public async Task Asignar_EmpleadoInactivo_LanzaConflict()
    {
        using var db = _fixture.CreateContext();
        var usuario = await TestData.UsuarioAsync(db);
        var empleado = await TestData.EmpleadoAsync(db, activo: false);
        var equipo = await TestData.EquipoAsync(db);
        var service = new AsignacionService(db);

        await Assert.ThrowsAsync<ConflictException>(() => service.AsignarAsync(
            new AsignarRequest { EquipoId = equipo.Id, EmpleadoId = empleado.Id },
            usuario.Id,
            CancellationToken.None));
    }

    [Theory]
    [InlineData(EstadoEquipo.Disponible)]
    [InlineData(EstadoEquipo.EnReparacion)]
    public async Task Devolver_DejaElEquipoEnElEstadoIndicado(EstadoEquipo estadoFinal)
    {
        using var db = _fixture.CreateContext();
        var usuario = await TestData.UsuarioAsync(db);
        var empleado = await TestData.EmpleadoAsync(db);
        var equipo = await TestData.EquipoAsync(db);
        var service = new AsignacionService(db);
        var asignacion = await service.AsignarAsync(
            new AsignarRequest { EquipoId = equipo.Id, EmpleadoId = empleado.Id },
            usuario.Id,
            CancellationToken.None);

        var resultado = await service.DevolverAsync(
            asignacion.Id,
            new DevolucionRequest { EstadoEquipo = estadoFinal },
            CancellationToken.None);

        Assert.False(resultado.Activa);
        Assert.NotNull(resultado.FechaDevolucion);

        using var verificacion = _fixture.CreateContext();
        var equipoGuardado = await verificacion.Equipos.FindAsync(equipo.Id);
        Assert.Equal(estadoFinal, equipoGuardado!.Estado);
    }

    [Fact]
    public async Task Devolver_DosVeces_LanzaConflict()
    {
        using var db = _fixture.CreateContext();
        var usuario = await TestData.UsuarioAsync(db);
        var empleado = await TestData.EmpleadoAsync(db);
        var equipo = await TestData.EquipoAsync(db);
        var service = new AsignacionService(db);
        var asignacion = await service.AsignarAsync(
            new AsignarRequest { EquipoId = equipo.Id, EmpleadoId = empleado.Id },
            usuario.Id,
            CancellationToken.None);

        await service.DevolverAsync(asignacion.Id, new DevolucionRequest(), CancellationToken.None);

        await Assert.ThrowsAsync<ConflictException>(
            () => service.DevolverAsync(asignacion.Id, new DevolucionRequest(), CancellationToken.None));
    }

    [Fact]
    public async Task Devolver_ConEstadoDeBaja_LanzaReglaNegocio()
    {
        using var db = _fixture.CreateContext();
        var service = new AsignacionService(db);

        await Assert.ThrowsAsync<ReglaNegocioException>(() => service.DevolverAsync(
            1,
            new DevolucionRequest { EstadoEquipo = EstadoEquipo.DeBaja },
            CancellationToken.None));
    }

    [Fact]
    public async Task BaseDeDatos_ImpideDosAsignacionesActivasDelMismoEquipo()
    {
        using var db = _fixture.CreateContext();
        var usuario = await TestData.UsuarioAsync(db);
        var empleado = await TestData.EmpleadoAsync(db);
        var equipo = await TestData.EquipoAsync(db);

        // Insertamos directamente, saltándonos el servicio, para probar el índice único
        db.Asignaciones.Add(new Asignacion { EquipoId = equipo.Id, EmpleadoId = empleado.Id, AsignadoPorId = usuario.Id });
        db.Asignaciones.Add(new Asignacion { EquipoId = equipo.Id, EmpleadoId = empleado.Id, AsignadoPorId = usuario.Id });

        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
    }
}