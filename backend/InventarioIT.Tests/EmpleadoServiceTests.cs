using InventarioIT.Api.Dtos;
using InventarioIT.Api.Exceptions;
using InventarioIT.Api.Models;
using InventarioIT.Api.Services;
using InventarioIT.Tests.Infraestructura;

namespace InventarioIT.Tests;

[Collection("Database")]
public class EmpleadoServiceTests
{
    private readonly DatabaseFixture _fixture;

    public EmpleadoServiceTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _fixture.LimpiarDatos();
    }

    [Fact]
    public async Task Crear_CorreoDuplicadoSinDistinguirMayusculas_LanzaConflict()
    {
        using var db = _fixture.CreateContext();
        await TestData.EmpleadoAsync(db, email: "ana@empresa.com");
        var service = new EmpleadoService(db);

        await Assert.ThrowsAsync<ConflictException>(() => service.CrearAsync(new EmpleadoRequest
        {
            Nombre = "Ana",
            Apellido = "Duplicada",
            Email = "ANA@Empresa.com",
            Departamento = "Ventas",
            Puesto = "Asesora"
        }, CancellationToken.None));
    }

    [Fact]
    public async Task Desactivar_ConEquiposAsignados_LanzaConflict()
    {
        using var db = _fixture.CreateContext();
        var usuario = await TestData.UsuarioAsync(db);
        var empleado = await TestData.EmpleadoAsync(db);
        var equipo = await TestData.EquipoAsync(db, estado: EstadoEquipo.Asignado);
        db.Asignaciones.Add(new Asignacion
        {
            EquipoId = equipo.Id,
            EmpleadoId = empleado.Id,
            AsignadoPorId = usuario.Id
        });
        await db.SaveChangesAsync();
        var service = new EmpleadoService(db);

        await Assert.ThrowsAsync<ConflictException>(
            () => service.DesactivarAsync(empleado.Id, CancellationToken.None));
    }

    [Fact]
    public async Task Desactivar_SinEquipos_QuedaInactivo()
    {
        using var db = _fixture.CreateContext();
        var empleado = await TestData.EmpleadoAsync(db);
        var service = new EmpleadoService(db);

        await service.DesactivarAsync(empleado.Id, CancellationToken.None);

        var resultado = await service.ObtenerAsync(empleado.Id, CancellationToken.None);
        Assert.False(resultado.Activo);
    }
}