using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace pigeon_api.SignalR.Hubs
{
    public sealed class NotificationsHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst("sub")?.Value;
            if (userId != null)
            {
                // Store the connection ID against the user (or use a service for managing this)
                // For example: _userConnections.Add(userId, Context.ConnectionId);
            }

            return base.OnConnectedAsync();
        }

        public async Task NotifyFriendshipRequested(string targetUserId, string requesterId)
        {
            // Send to a specific user, or all users
            await Clients.User(targetUserId).SendAsync("FriendshipRequested", new
            {
                requesterId,
                message = $"{requesterId} has requested to be your friend."
            });
        }
    }
}
