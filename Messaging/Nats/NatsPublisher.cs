using System.Text;
using System.Text.Json;
using NATS.Client.JetStream;
using pigeon_api.Messaging.Contracts;

namespace pigeon_api.Messaging.Nats;

public sealed class NatsPublisher
{
    private readonly IJetStream _js;

    public NatsPublisher(NatsConnection connection)
    {
        _js = connection.Connection.CreateJetStreamContext();
    }

    // Generic low-level publish
    public void Publish<T>(string subject, T payload)
    {
        var json = JsonSerializer.Serialize(payload);
        var data = Encoding.UTF8.GetBytes(json);

        _js.Publish(subject, data);

        Console.WriteLine(
        $"[NATS:PUBLISH] Subject={subject} Payload={json}"
    );
    }

    // Domain-specific publish 
    public void PublishFriendshipCreated(
        int UserId,
        int FriendId,
        DateTime CreatedAt
        )
    {
        var evt = new FriendshipCreatedEvent(
            UserId,
            FriendId,
            CreatedAt
        );

        Publish(
            Subjects.FriendshipCreated,
            evt
        );
    }
}
