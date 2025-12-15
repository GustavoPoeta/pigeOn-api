using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NATS.Client;
using NATS.Client.JetStream;
using System.Text;
using pigeon_api.Messaging.Nats;
using Microsoft.AspNetCore.SignalR;
using pigeon_api.SignalR.Hubs;
using System.Text.Json;
using pigeon_api.Messaging.Contracts;

namespace pigeon_api.Messaging.Consumers;

public sealed class FriendshipRequestedConsumer : BackgroundService
{
    private readonly IJetStream _js;
    private readonly ILogger<FriendshipRequestedConsumer> _logger;
    private readonly IHubContext<NotificationsHub> _hubContext;

    public FriendshipRequestedConsumer(
        NatsConnection connection,
        ILogger<FriendshipRequestedConsumer> logger,
        IHubContext<NotificationsHub> hubContext)
    {
        _js = connection.Connection.CreateJetStreamContext();
        _logger = logger;
        _hubContext = hubContext;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {

        var consumerConfig = ConsumerConfiguration.Builder()
            .WithDurable("friendship-consumer")
            .WithAckPolicy(AckPolicy.Explicit)
            .Build();

        var options = PushSubscribeOptions.Builder()
            .WithConfiguration(consumerConfig)
            .Build();

        _js.PushSubscribeAsync(
            Subjects.FriendshipRequested,
            async (sender, args) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(args.Message.Data);

                    var evt = JsonSerializer.Deserialize<FriendshipCreatedEvent>(json);

                    if (evt is not null)
                    {

                        if (evt.FriendId <= 0)
                        {
                            _logger.LogWarning("Invalid FriendId, skipping SignalR push");
                            args.Message.Ack();
                            return;
                        }

                        await _hubContext.Clients
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
                    args.Message.Ack();

                    _logger.LogInformation(
                        "FriendshipCreated event ACKed. StreamSeq={StreamSeq}",
                        args.Message.MetaData?.StreamSequence
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing FriendshipCreated event");
                    args.Message.Ack();
                }
            },
            autoAck: false,
            options
        );


        _logger.LogInformation(
            "FriendshipRequestedConsumer subscribed successfully."
        );

        return Task.CompletedTask;
    }
}
