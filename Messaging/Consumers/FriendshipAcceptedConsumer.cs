using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using NATS.Client.JetStream;
using pigeon_api.Messaging.Contracts;
using pigeon_api.Messaging.Nats;
using pigeon_api.SignalR.Hubs;

namespace pigeon_api.Messaging.Consumers;

public sealed class FriendshipAcceptedConsumer : BackgroundService
{
    private readonly IJetStream _js;
    private readonly ILogger<FriendshipRequestedConsumer> _logger;
    private readonly IHubContext<NotificationsHub> _hubContext;

    public FriendshipAcceptedConsumer(
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
            Subjects.FriendshipAccepted,
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
                                "FriendshipAccepted",
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
            "FriendshipAcceptedConsumer subscribed successfully."
        );

        return Task.CompletedTask;
    }
}