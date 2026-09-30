using InventarioIT.Api.Data;
using InventarioIT.Api.Dtos;
using InventarioIT.Api.Exceptions;
using InventarioIT.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InventarioIT.Api.Services;

public class AuthService(
    AppDbContext db,
    IPasswordHasher<Usuario> hasher,
    ITokenService tokenService) : IAuthService
{
    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var usuario = await db.Usuarios.FirstOrDefaultAsync(u => u.Email == email, ct);

        // Mismo mensaje para todos los casos: no revelamos si el correo existe
        const string mensaje = "Correo o contraseña incorrectos.";

        if (usuario is null || !usuario.Activo)
        {
            throw new UnauthorizedException(mensaje);
        }

        var resultado = hasher.VerifyHashedPassword(usuario, usuario.PasswordHash, request.Password);

        if (resultado == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedException(mensaje);
        }

        var (token, expiraEn) = tokenService.GenerarToken(usuario);

        return new LoginResponse(token, expiraEn, UsuarioResponse.Desde(usuario));
    }
}