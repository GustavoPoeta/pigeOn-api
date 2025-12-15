namespace pigeon_api.Messaging.Contracts;

public sealed record FriendshipCreatedEvent(
    int UserId,
    int FriendId,
    Boolean SaveOnCache,
    DateTime CreatedAt
);
