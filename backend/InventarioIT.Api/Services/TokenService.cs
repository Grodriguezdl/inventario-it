using System.Security.Claims;
using System.Text;
using InventarioIT.Api.Models;
using InventarioIT.Api.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace InventarioIT.Api.Services;

public class TokenService(IOptions<JwtOptions> options) : ITokenService
{
    private readonly JwtOptions _jwt = options.Value;

    public (string Token, DateTime ExpiraEn) GenerarToken(Usuario usuario)
    {
        var expiraEn = DateTime.UtcNow.AddMinutes(_jwt.ExpiraMinutos);
        var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwt.Issuer,
            Audience = _jwt.Audience,
            Expires = expiraEn,
            SigningCredentials = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim("name", usuario.Nombre),
                new Claim("role", usuario.Rol.ToString())
            ])
        };

        return (new JsonWebTokenHandler().CreateToken(descriptor), expiraEn);
    }
}