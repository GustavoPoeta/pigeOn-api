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
    private readonly ILogger<FriendshipEventsConsumer> _logger;
    private readonly IHubContext<NotificationsHub> _hubContext;
    private readonly IServiceScopeFactory _serviceProvider;
    private IJetStreamPushAsyncSubscription _subscription;

    public FriendshipEventsConsumer(
        NatsConnection connection,
        ILogger<FriendshipEventsConsumer> logger,
        IHubContext<NotificationsHub> hubContext,
        IServiceScopeFactory serviceProvider)
    {
        _js = connection.Connection.CreateJetStreamContext();
        _logger = logger;
        _hubContext = hubContext;
        _serviceProvider = serviceProvider;
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
                                await FriendshipHandlers.FriendshipRequestedHandler(args, _hubContext, requestedEvent, _serviceProvider);
                                break;

                            case Subjects.FriendshipAccepted:
                                var acceptedEvent = JsonSerializer.Deserialize<FriendshipCreatedEvent>(json);
                                await FriendshipHandlers.FriendshipAcceptedHandler(args, _hubContext, acceptedEvent);
                                break;
                        }

                        args.Message.Ack();
                        break; // Exit retry loop on success
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

        _logger.LogInformation("FriendshipEventsConsumer started successfully");
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping FriendshipEventsConsumer...");
        
        if (_subscription != null)
        {
            _subscription.Unsubscribe();
            _subscription.Dispose();
        }
        
        await base.StopAsync(cancellationToken);
    }
}