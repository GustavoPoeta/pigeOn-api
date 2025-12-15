using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using NATS.Client.JetStream;
using pigeon_api.Messaging.Contracts;
using pigeon_api.Messaging.Nats;
using pigeon_api.SignalR.Hubs;
using pigeon_api.Messaging.ConsumerHandlers;

namespace pigeon_api.Messaging.Consumers;

public sealed class FriendshipEventsConsumer : BackgroundService
{
    private readonly IJetStream _js;
    private readonly ILogger _logger;
    private IJetStreamPushAsyncSubscription _subscription;
    private readonly IHubContext<NotificationsHub> _hubContext;

    public FriendshipEventsConsumer(
        NatsConnection connection,
        ILogger logger,
        IJetStreamPushAsyncSubscription subscription,
        IHubContext<NotificationsHub> hubContext)
    {
        _js = connection.Connection.CreateJetStreamContext();
        _logger = logger;
        _subscription = subscription;
        _hubContext = hubContext;
    }


    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumerConfig = ConsumerConfiguration.Builder()
            .WithDurable("friendship-events-consumer")
            .WithAckPolicy(AckPolicy.Explicit)
            .Build();

        var options = PushSubscribeOptions.Builder()
            .WithConfiguration(consumerConfig)
            .Build();

        _subscription = _js.PushSubscribeAsync(
            "friendship.*",
            async (sender, args) =>
            {
                var maxRetries = 3;
                var retryCount = 0;

                while (retryCount < maxRetries)
                {
                    try
                    {
                        var json = Encoding.UTF8.GetString(args.Message.Data);

                        // Route based on subject
                        switch (args.Message.Subject)
                        {
                            case Subjects.FriendshipRequested:
                                var requestedEvent = JsonSerializer.Deserialize<FriendshipCreatedEvent>(json);
                                await FriendshipHandlers.FriendshipRequestedHandler(args, _hubContext, requestedEvent);
                                break;

                            case Subjects.FriendshipAccepted:
                                var acceptedEvent = JsonSerializer.Deserialize<FriendshipCreatedEvent>(json);
                                await FriendshipHandlers.FriendshipAcceptedHandler(args, _hubContext, acceptedEvent);
                                break;
                        }

                        args.Message.Ack();
                    }
                    catch (Exception ex)
                    {
                        retryCount++;
                        if (retryCount >= maxRetries)
                        {
                            _logger.LogError(ex, "Failed to process message after {Retries} attempts", maxRetries);
                            args.Message.Nak();
                        }
                        else
                        {
                            await Task.Delay(1000 * retryCount);
                        }
                    }
                }
            },
            autoAck: false,
            options
        );

        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _subscription?.Unsubscribe();
        _subscription?.Dispose();
        await base.StopAsync(cancellationToken);
    }
}