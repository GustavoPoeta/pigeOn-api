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
    public void PublishFriendshipRequested(
        int UserId,
        int FriendId,
        Boolean SaveOnCache,
        DateTime CreatedAt
        )
    {
        var evt = new FriendshipCreatedEvent(
            UserId,
            FriendId,
            SaveOnCache,
            CreatedAt
        );

        Publish(
            Subjects.FriendshipRequested,
            evt
        );
    }

    public void PublishFriendshipAccepted(
        int UserId,
        int  FriendId,
        Boolean SaveOnCache,
        DateTime CreatedAt
    )
    {
        var evt = new FriendshipCreatedEvent(
            UserId,
            FriendId,
            SaveOnCache,
            CreatedAt
        );

        Publish(
            Subjects.FriendshipAccepted,
            evt
        );
    }

    public void PublishMessageCreated(
        int SenderId,
        int ReceiverId,
        string Content,
        DateTime CreatedAt
    )
    {
        var evt = new MessageCreatedEvent(
            SenderId,
            ReceiverId,
            Content,
            CreatedAt
        );

        Publish(
            Subjects.MessageCreated,
            evt
        );
    }
}
