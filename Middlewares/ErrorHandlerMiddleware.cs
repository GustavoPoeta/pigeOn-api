using pigeon_api.Models;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ErrorHandlerMiddleware(RequestDelegate next,
                                   ILogger<ErrorHandlerMiddleware> logger,
                                   IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ApiException ex)
        {
            _logger.LogWarning(ex, "Handled API exception");

            context.Response.StatusCode = ex.StatusCode;
            context.Response.ContentType = "application/json";

            var response = new ErrorResponse(
                ex.StatusCode,
                ex.Message
            );

            await context.Response.WriteAsJsonAsync(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new ErrorResponse(
                StatusCodes.Status500InternalServerError,
                _env.IsDevelopment()
                    ? ex.Message
                    : "An unexpected error occurred"
            );

            await context.Response.WriteAsJsonAsync(response);
        }
    }


}