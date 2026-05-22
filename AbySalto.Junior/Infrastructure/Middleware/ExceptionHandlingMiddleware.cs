using System.Text.Json;
using AbySalto.Junior.Application.Exceptions;

namespace AbySalto.Junior.Infrastructure.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException exception)
        {
            await WriteErrorResponseAsync(context, StatusCodes.Status404NotFound, exception.Message);
        }
        catch (BadRequestException exception)
        {
            await WriteErrorResponseAsync(context, StatusCodes.Status400BadRequest, exception.Message);
        }
        catch (ConflictException exception)
        {
            await WriteErrorResponseAsync(context, StatusCodes.Status409Conflict, exception.Message);
        }
        catch (ForbiddenException exception)
        {
            await WriteErrorResponseAsync(context, StatusCodes.Status403Forbidden, exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception while processing {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            var message = _environment.IsDevelopment()
                ? exception.Message
                : "An unexpected error occurred.";

            await WriteErrorResponseAsync(context, StatusCodes.Status500InternalServerError, message);
        }
    }

    private static async Task WriteErrorResponseAsync(HttpContext context, int statusCode, string message)
    {
        if (context.Response.HasStarted)
        {
            throw new InvalidOperationException("The response has already started.");
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(new { message }));
    }
}
