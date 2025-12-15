namespace pigeon_api.Messaging.Contracts;

public sealed record MessageCreatedEvent (
    int SenderId,
    int ReceiverId,
    string Content,
    DateTime CreatedAt
);