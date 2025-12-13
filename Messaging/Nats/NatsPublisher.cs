using System.Text;
using System.Text.Json;
using NATS.Client;
using NATS.Client.JetStream;

namespace pigeon_api.Messaging.Nats;

public sealed class NatsPublisher
{
    private readonly IJetStream _js;

    public NatsPublisher(NatsConnection connection)
    {
        _js = connection.Connection.CreateJetStreamContext();
    }

    public void Publish<T>(string subject, T payload)
    {
        var json = JsonSerializer.Serialize(payload);
        var data = Encoding.UTF8.GetBytes(json);

        _js.Publish(subject, data);
    }
}
