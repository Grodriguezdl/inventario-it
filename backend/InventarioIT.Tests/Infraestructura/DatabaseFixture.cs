using InventarioIT.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace InventarioIT.Tests.Infraestructura;

// Se crea UNA vez para todas las pruebas: prepara la base de datos de pruebas
public class DatabaseFixture : IDisposable
{
    private readonly string _connectionString;

    public DatabaseFixture()
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<DatabaseFixture>(optional: true) // en tu computadora
            .AddEnvironmentVariables()                       // en GitHub Actions
            .Build();

        _connectionString = config.GetConnectionString("Tests")
            ?? throw new InvalidOperationException(
                "Falta ConnectionStrings:Tests. Configúrala con dotnet user-secrets o una variable de entorno.");

        using var db = CreateContext();
        db.Database.EnsureDeleted();
        db.Database.Migrate(); // aplica las migraciones reales, así también se prueban
    }

    public AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_connectionString)
            .Options;

        return new AppDbContext(options);
    }

    // Deja las tablas vacías antes de cada prueba (las categorías se conservan)
    public void LimpiarDatos()
    {
        using var db = CreateContext();
        db.Database.ExecuteSqlRaw(
            "TRUNCATE TABLE \"Asignaciones\", \"Equipos\", \"Empleados\", \"Usuarios\" RESTART IDENTITY CASCADE;");
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}

// Todas las clases de prueba comparten la base, así que se ejecutan una tras otra
[CollectionDefinition("Database")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture>;