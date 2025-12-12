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
            // Continue the pipeline
            await _next(context);
        }
        catch (Exception ex)
        {
            // Log the error
            _logger.LogError(ex, "Unhandled exception occurred");

            // Prepare the response
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new ErrorResponse (context.Response.StatusCode, "An unexpected error has ocurred");
            
            await context.Response.WriteAsJsonAsync(response);
        }
    }
 
}