using Microsoft.AspNetCore.SignalR;

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
            await Clients.User(targetUserId).SendAsync("FriendshipRequested", new
            {
                requesterId,
                message = $"{requesterId} has requested to be your friend."
            });
        }

        public async Task NotifyFriendshipAccepted(string targetUserId, string userAcceptedId)
        {
            await Clients.User(targetUserId).SendAsync("FriendshipAccepted", new
            {
               userAcceptedId,
               message = $"{userAcceptedId} has accepted to be your friend."
            });
        }

        public async Task NotifyMessageCreated(string targetUserId, string senderId)
        {
            await Clients.User(targetUserId).SendAsync("MessageCreated", new
            {
                senderId,
                message = $"{senderId} has sent you a message."
            });
        }
    }
}
