using Microsoft.AspNetCore.SignalR;
using NATS.Client;
using pigeon_api.Contexts;
using pigeon_api.Enums;
using pigeon_api.Messaging.Contracts;
using pigeon_api.Models;
using pigeon_api.SignalR;
using pigeon_api.SignalR.Hubs;

namespace pigeon_api.Messaging.ConsumerHandlers;

public sealed class FriendshipHandlers
{
    public static async Task FriendshipRequestedHandler(MsgHandlerEventArgs args, IHubContext<NotificationsHub> hub, FriendshipCreatedEvent? evt, IServiceScopeFactory serviceProvider)
    {
        if (evt is not null)
        {
            if (evt.FriendId <= 0)
            {
                args.Message.Ack();
                return;
            }

            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var notification = new Notification
                {
                    UserId = evt.FriendId,
                    Type = NotificationTypes.FriendshipRequest,
                    Content = "A friendship request has been sent to you.",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow,
                    ReadAt = null,
                    FromUserId = evt.UserId,
                };

                await dbContext.Notifications.AddAsync(notification);
                await dbContext.SaveChangesAsync();
            }

            await hub.Clients
                .User(evt.FriendId.ToString())
                .SendAsync(
                    "FriendshipRequested",
                    new
                    {
                        fromUserId = evt.UserId,
                        createdAt = evt.CreatedAt,
                    }
                );
        }
    }

    public static async Task FriendshipAcceptedHandler(MsgHandlerEventArgs args, IHubContext<NotificationsHub> hub, FriendshipCreatedEvent? evt)
    {
        if (evt is not null)
        {
            if (evt.FriendId <= 0)
            {
                args.Message.Ack();
                return;
            }

            await hub.Clients
                .User(evt.FriendId.ToString())
                .SendAsync(
                    "FriendshipAccepted",
                    new
                    {
                        fromUserId = evt.UserId,
                        createdAt = evt.CreatedAt,
                    }
                );
        }
    }
}