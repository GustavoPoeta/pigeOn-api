using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NATS.Client;
using NATS.Client.JetStream;
using System.Text;
using pigeon_api.Messaging.Nats;

namespace pigeon_api.Messaging.Consumers;

public sealed class FriendshipCreatedConsumer : BackgroundService
{
    private readonly IJetStream _js;
    private readonly ILogger<FriendshipCreatedConsumer> _logger;

    public FriendshipCreatedConsumer(
        NatsConnection connection,
        ILogger<FriendshipCreatedConsumer> logger)
    {
        _js = connection.Connection.CreateJetStreamContext();
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Starting FriendshipCreatedConsumer. Subject={Subject}",
            Subjects.FriendshipCreated
        );

        var consumerConfig = ConsumerConfiguration.Builder()
            .WithDurable("friendship-consumer")
            .WithAckPolicy(AckPolicy.Explicit)
            .Build();

        var options = PushSubscribeOptions.Builder()
            .WithConfiguration(consumerConfig)
            .Build();

        _js.PushSubscribeAsync(
            Subjects.FriendshipCreated,
            (sender, args) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(args.Message.Data);

                    _logger.LogInformation(
                        "FriendshipCreated event received. StreamSeq={StreamSeq} Payload={Payload}",
                        args.Message.MetaData?.StreamSequence,
                        json
                    );

                    // Side effects only:
                    // - SignalR
                    // - Notifications
                    // - Cache updates

                    args.Message.Ack();

                    _logger.LogInformation(
                        "FriendshipCreated event ACKed. StreamSeq={StreamSeq}",
                        args.Message.MetaData?.StreamSequence
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing FriendshipCreated event");
                }
            },
            autoAck: false,
            options
        );


        _logger.LogInformation(
            "FriendshipCreatedConsumer subscribed successfully."
        );

        return Task.CompletedTask;
    }
}
