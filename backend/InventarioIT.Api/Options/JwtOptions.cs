namespace InventarioIT.Api.Options;

public class JwtOptions
{
    public const string Seccion = "Jwt";

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public int ExpiraMinutos { get; set; } = 60;
}