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

        var jsm = Connection.CreateJetStreamManagementContext();

        EnsureStreams(jsm, "FRIENDSHIPS", "friendship.*");
        EnsureStreams(jsm, "MESSAGES", "message.*");
    }

    private void EnsureStreams(IJetStreamManagement jsm, string name, string subject)
    {

        try
        {
            jsm.GetStreamInfo(name);
        }
        catch (NATSJetStreamException)
        {
            jsm.AddStream(StreamConfiguration.Builder()
                .WithName(name)
                .WithSubjects(subject)
                .WithStorageType(StorageType.File)
                .Build());
        }
    }

    public void Dispose()
    {
        Connection?.Dispose();
    }
}
