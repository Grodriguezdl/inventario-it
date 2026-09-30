using InventarioIT.Api.Models;

namespace InventarioIT.Api.Services;

public interface ITokenService
{
    (string Token, DateTime ExpiraEn) GenerarToken(Usuario usuario);
}