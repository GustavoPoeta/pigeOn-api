using System.Security.Claims;
using System.Text.Json;

namespace pigeon_api.Middlewares;

public sealed class UserAuthMiddleware
{
    private readonly RequestDelegate _next;

    public UserAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string? userId = null;

        // 1. Normal HTTP requests
        if (context.Request.Headers.TryGetValue("X-User-Id", out var headerUserId))
        {
            userId = headerUserId.FirstOrDefault();
        }

        // 2. SignalR transport requests
        if (string.IsNullOrEmpty(userId))
        {
            var accessToken = context.Request.Query["userId"].FirstOrDefault();

            
            if (!string.IsNullOrEmpty(accessToken))
            {
                userId = accessToken;
            }
        }

        if (!string.IsNullOrWhiteSpace(userId))
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, userId)
        };

            context.User = new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Custom")
            );
        }

        await _next(context);
    }

    record AuthPayload(string UserId, string ApiKey);
}
