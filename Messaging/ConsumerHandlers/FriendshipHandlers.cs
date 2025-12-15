using Microsoft.AspNetCore.SignalR;
using NATS.Client;
using pigeon_api.Messaging.Contracts;
using pigeon_api.SignalR.Hubs;

namespace pigeon_api.Messaging.ConsumerHandlers;

public sealed class FriendshipHandlers
{
    public static async Task FriendshipRequestedHandler(MsgHandlerEventArgs args, IHubContext<NotificationsHub> hub, FriendshipCreatedEvent? evt)
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