using NATS.Client;
using NATS.Client.JetStream;

namespace pigeon_api.Messaging.Nats;

public sealed class NatsConnection : IDisposable
{
    public IConnection Connection { get; }

    public NatsConnection(IConfiguration config)
    {
        var opts = ConnectionFactory.GetDefaultOptions();
        opts.Url = config["Nats:Url"] ?? "nats://localhost:4222";

        Connection = new ConnectionFactory().CreateConnection(opts);

        EnsureStreams();
    }

    private void EnsureStreams()
    {
        var jsm = Connection.CreateJetStreamManagementContext();

        try
        {
            jsm.GetStreamInfo("FRIENDSHIPS");
        }
        catch (NATSJetStreamException)
        {
            jsm.AddStream(StreamConfiguration.Builder()
                .WithName("FRIENDSHIPS")
                .WithSubjects("friendship.*")
                .WithStorageType(StorageType.File)
                .Build());
        }
    }

    public void Dispose()
    {
        Connection?.Dispose();
    }
}
