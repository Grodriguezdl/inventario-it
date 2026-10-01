using InventarioIT.Api.Data;
using InventarioIT.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace InventarioIT.Tests.Infraestructura;

// Atajos para crear datos de prueba con valores por defecto
public static class TestData
{
    public const string PasswordPorDefecto = "Password123";

    public static async Task<Usuario> UsuarioAsync(
        AppDbContext db,
        string email = "admin@test.local",
        RolUsuario rol = RolUsuario.Admin,
        bool activo = true)
    {
        var usuario = new Usuario { Nombre = "Usuario Prueba", Email = email, Rol = rol, Activo = activo };
        usuario.PasswordHash = new PasswordHasher<Usuario>().HashPassword(usuario, PasswordPorDefecto);

        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();
        return usuario;
    }

    public static async Task<Empleado> EmpleadoAsync(
        AppDbContext db,
        string email = "empleado@test.local",
        bool activo = true)
    {
        var empleado = new Empleado
        {
            Nombre = "Ana",
            Apellido = "Prueba",
            Email = email,
            Departamento = "Finanzas",
            Puesto = "Analista",
            Activo = activo
        };

        db.Empleados.Add(empleado);
        await db.SaveChangesAsync();
        return empleado;
    }

    public static async Task<Equipo> EquipoAsync(
        AppDbContext db,
        string codigo = "LAP-001",
        string marca = "Dell",
        EstadoEquipo estado = EstadoEquipo.Disponible,
        int categoriaId = 1)
    {
        var equipo = new Equipo
        {
            CodigoInventario = codigo,
            Marca = marca,
            Modelo = "Modelo Prueba",
            Estado = estado,
            CategoriaId = categoriaId
        };

        db.Equipos.Add(equipo);
        await db.SaveChangesAsync();
        return equipo;
    }
}