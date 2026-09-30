using InventarioIT.Api.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace InventarioIT.Api.Middleware;

public class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, titulo) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Recurso no encontrado"),
            ConflictException => (StatusCodes.Status409Conflict, "Conflicto"),
            ReglaNegocioException => (StatusCodes.Status400BadRequest, "Solicitud inválida"),
            UnauthorizedException => (StatusCodes.Status401Unauthorized, "No autorizado"),
            _ => (StatusCodes.Status500InternalServerError, "Error interno del servidor")
        };

        if (status == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Error no controlado");
        }

        httpContext.Response.StatusCode = status;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Title = titulo,
                // En errores 500 no mostramos el detalle interno al cliente
                Detail = status == StatusCodes.Status500InternalServerError
                    ? "Ocurrió un error inesperado."
                    : exception.Message
            }
        });
    }
}