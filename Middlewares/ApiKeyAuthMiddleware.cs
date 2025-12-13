using System.Text.Json;

namespace pigeon_api.Middlewares;

public sealed class ApiKeyAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public ApiKeyAuthMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string? apiKey = null;

        // 1. Try to get token from the Authorization Header (Used during 'negotiate' HTTP request)
        string? token = null;
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            token = authHeader.Substring("Bearer ".Length).Trim();
        }

        // 2. If not in header, try Query String (Used during WebSocket connection)
        if (string.IsNullOrEmpty(token))
        {
            token = context.Request.Query["access_token"].FirstOrDefault();
        }

        // 3. Extract the API Key
        if (!string.IsNullOrEmpty(token))
        {
            apiKey = token; 
        }

        // 4. Validate API key
        var configuredApiKey = _configuration.GetValue<string>("ApiKeys:MyApi");
        
        // Safety check for empty configuration to prevent accidental access if config is missing
        if (string.IsNullOrEmpty(configuredApiKey))
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync("Server configuration error");
            return;
        }

        if (string.IsNullOrEmpty(apiKey) || !string.Equals(apiKey, configuredApiKey))
        {
            Console.WriteLine($"Auth Failed. Received: {apiKey}");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Invalid or missing API Key in token");
            return;
        }

        await _next(context);
    }
}