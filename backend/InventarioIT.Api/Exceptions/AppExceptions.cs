namespace InventarioIT.Api.Exceptions;

// 404: el recurso no existe
public class NotFoundException(string message) : Exception(message);

// 409: la operación choca con el estado actual (código duplicado, equipo asignado...)
public class ConflictException(string message) : Exception(message);

// 400: los datos no cumplen una regla de negocio (categoría inexistente...)
public class ReglaNegocioException(string message) : Exception(message);
// 401: credenciales inválidas o token sin datos válidos
public class UnauthorizedException(string message) : Exception(message);
