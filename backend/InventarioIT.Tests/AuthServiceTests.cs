using InventarioIT.Api.Dtos;
using InventarioIT.Api.Exceptions;
using InventarioIT.Api.Models;
using InventarioIT.Api.Options;
using InventarioIT.Api.Services;
using InventarioIT.Tests.Infraestructura;
using Microsoft.AspNetCore.Identity;

namespace InventarioIT.Tests;

[Collection("Database")]
public class AuthServiceTests
{
    private readonly DatabaseFixture _fixture;

    public AuthServiceTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _fixture.LimpiarDatos();
    }

    private static AuthService CrearServicio(Api.Data.AppDbContext db)
    {
        var jwt = Microsoft.Extensions.Options.Options.Create(new JwtOptions
        {
            Issuer = "pruebas",
            Audience = "pruebas",
            Key = new string('k', 64),
            ExpiraMinutos = 60
        });

        return new AuthService(db, new PasswordHasher<Usuario>(), new TokenService(jwt));
    }

    [Fact]
    public async Task Login_ConCredencialesCorrectas_DevuelveToken()
    {
        using var db = _fixture.CreateContext();
        await TestData.UsuarioAsync(db, email: "admin@test.local");
        var service = CrearServicio(db);

        // El correo en mayúsculas también debe funcionar
        var resultado = await service.LoginAsync(
            new LoginRequest { Email = "ADMIN@test.local", Password = TestData.PasswordPorDefecto },
            CancellationToken.None);

        Assert.False(string.IsNullOrWhiteSpace(resultado.Token));
        Assert.Equal(RolUsuario.Admin, resultado.Usuario.Rol);
    }

    [Fact]
    public async Task Login_ConPasswordIncorrecta_LanzaUnauthorized()
    {
        using var db = _fixture.CreateContext();
        await TestData.UsuarioAsync(db, email: "admin@test.local");
        var service = CrearServicio(db);

        await Assert.ThrowsAsync<UnauthorizedException>(() => service.LoginAsync(
            new LoginRequest { Email = "admin@test.local", Password = "incorrecta" },
            CancellationToken.None));
    }

    [Fact]
    public async Task Login_UsuarioInactivo_LanzaUnauthorized()
    {
        using var db = _fixture.CreateContext();
        await TestData.UsuarioAsync(db, email: "inactivo@test.local", activo: false);
        var service = CrearServicio(db);

        await Assert.ThrowsAsync<UnauthorizedException>(() => service.LoginAsync(
            new LoginRequest { Email = "inactivo@test.local", Password = TestData.PasswordPorDefecto },
            CancellationToken.None));
    }

    [Fact]
    public async Task Login_CorreoInexistente_LanzaUnauthorized()
    {
        using var db = _fixture.CreateContext();
        var service = CrearServicio(db);

        await Assert.ThrowsAsync<UnauthorizedException>(() => service.LoginAsync(
            new LoginRequest { Email = "nadie@test.local", Password = "loquesea" },
            CancellationToken.None));
    }
}