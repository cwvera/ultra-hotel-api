using System.Text.Json;
using FluentValidation;

namespace UltraHotel.WebApi.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode  = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            await WriteAsync(context, new
            {
                message = "Error de validación.",
                errors  = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            context.Response.StatusCode  = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await WriteAsync(context, new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            context.Response.StatusCode  = StatusCodes.Status409Conflict;
            context.Response.ContentType = "application/json";
            await WriteAsync(context, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error interno no controlado.");
            context.Response.StatusCode  = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            await WriteAsync(context, new { message = "Error interno del servidor." });
        }
    }

    private static Task WriteAsync(HttpContext context, object body) =>
        context.Response.WriteAsync(JsonSerializer.Serialize(body, JsonOptions));
}
