using InventarioIT.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InventarioIT.Api.Data;

public static class DataSeeder
{
    public static async Task CrearAdminInicialAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<Usuario>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DataSeeder");

        if (await db.Usuarios.AnyAsync())
        {
            return;
        }

        var email = config["Seed:AdminEmail"];
        var password = config["Seed:AdminPassword"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning("No hay usuarios y falta configurar Seed:AdminEmail y Seed:AdminPassword.");
            return;
        }

        var admin = new Usuario
        {
            Nombre = "Administrador",
            Email = email.Trim().ToLowerInvariant(),
            Rol = RolUsuario.Admin
        };
        admin.PasswordHash = hasher.HashPassword(admin, password);

        db.Usuarios.Add(admin);
        await db.SaveChangesAsync();

        logger.LogInformation("Administrador inicial creado: {Email}", admin.Email);
    }
}