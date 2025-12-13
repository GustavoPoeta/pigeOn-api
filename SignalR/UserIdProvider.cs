using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace pigeon_api.SignalR;

public sealed class UserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        // 1. Get the HTTP Context from the connection
        var httpContext = connection.GetHttpContext();

        // 2. Try to read the 'userId' from the query string
        if (httpContext?.Request.Query.TryGetValue("userId", out var userIdValue) == true)
        {
            // SignalR uses the first value if multiple exist
            return userIdValue.FirstOrDefault();
        }

        // 3. Fallback: Check standard claims if you decide to use JWT/Cookie Auth later
        // return connection.User?.FindFirst("sub")?.Value;

        // If no ID is found, the connection is considered anonymous (or unauthorized, 
        // depending on your middleware)
        return null;
    }
}
