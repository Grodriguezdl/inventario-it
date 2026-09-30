using System.Security.Claims;
using InventarioIT.Api.Exceptions;
using Microsoft.IdentityModel.JsonWebTokens;

namespace InventarioIT.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int ObtenerUsuarioId(this ClaimsPrincipal usuario)
    {
        var valor = usuario.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        return int.TryParse(valor, out var id)
            ? id
            : throw new UnauthorizedException("El token no contiene un usuario válido.");
    }
}