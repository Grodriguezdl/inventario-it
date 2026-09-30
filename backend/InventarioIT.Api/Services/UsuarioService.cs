using InventarioIT.Api.Data;
using InventarioIT.Api.Dtos;
using InventarioIT.Api.Exceptions;
using InventarioIT.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InventarioIT.Api.Services;

public class UsuarioService(AppDbContext db, IPasswordHasher<Usuario> hasher) : IUsuarioService
{
    public async Task<IReadOnlyList<UsuarioResponse>> ListarAsync(CancellationToken ct)
    {
        return await db.Usuarios
            .AsNoTracking()
            .OrderBy(u => u.Nombre)
            .Select(u => new UsuarioResponse(u.Id, u.Nombre, u.Email, u.Rol, u.Activo))
            .ToListAsync(ct);
    }

    public async Task<UsuarioResponse> ObtenerAsync(int id, CancellationToken ct)
    {
        var usuario = await db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct)
            ?? throw new NotFoundException($"No existe un usuario con Id {id}.");

        return UsuarioResponse.Desde(usuario);
    }

    public async Task<UsuarioResponse> CrearAsync(CrearUsuarioRequest request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (await db.Usuarios.AnyAsync(u => u.Email == email, ct))
        {
            throw new ConflictException($"Ya existe un usuario con el correo {email}.");
        }

        var usuario = new Usuario
        {
            Nombre = request.Nombre.Trim(),
            Email = email,
            Rol = request.Rol
        };
        usuario.PasswordHash = hasher.HashPassword(usuario, request.Password);

        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync(ct);

        return UsuarioResponse.Desde(usuario);
    }
}