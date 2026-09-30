using System.ComponentModel.DataAnnotations;
using InventarioIT.Api.Models;

namespace InventarioIT.Api.Dtos;

public class LoginRequest
{
    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    public string Password { get; set; } = string.Empty;
}

public record UsuarioResponse(int Id, string Nombre, string Email, RolUsuario Rol, bool Activo)
{
    public static UsuarioResponse Desde(Usuario u) => new(u.Id, u.Nombre, u.Email, u.Rol, u.Activo);
}

public record LoginResponse(string Token, DateTime ExpiraEn, UsuarioResponse Usuario);

public class CrearUsuarioRequest
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
    public string Password { get; set; } = string.Empty;

    public RolUsuario Rol { get; set; } = RolUsuario.Tecnico;
}