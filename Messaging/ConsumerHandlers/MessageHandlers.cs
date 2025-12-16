using Microsoft.AspNetCore.SignalR;
using NATS.Client;
using pigeon_api.Contexts;
using pigeon_api.Enums;
using pigeon_api.Messaging.Contracts;
using pigeon_api.Models;
using pigeon_api.SignalR.Hubs;

namespace pigeon_api.Messaging.ConsumerHandlers;

public sealed class MessageHandlers
{
    public static async Task MessageCreatedHandler(MsgHandlerEventArgs args, IHubContext<NotificationsHub> hub, MessageCreatedEvent? evt, IServiceScopeFactory serviceProvider)
    {
        if (evt is not null && (evt.Content.Length != 0 || evt.Content != null))
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var notification = new Notification
                {
                    UserId = evt.ReceiverId,
                    Type = NotificationTypes.MessageCreated,
                    Content = evt.Content,
                    FromUserId = evt.SenderId,
                    CreatedAt = DateTime.UtcNow
                };

                await dbContext.Notifications.AddAsync(notification);
                await dbContext.SaveChangesAsync();
            }

            await hub.Clients
                .User(evt.ReceiverId.ToString())
                .SendAsync(
                  "MessageCreated",
                  new
                  {
                      fromUserId = evt.SenderId,
                      content = evt.Content,
                      createdAt = evt.CreatedAt,
                  }  
                );

        } else
        {
            args.Message.Ack();
            return;
        }
    }

}