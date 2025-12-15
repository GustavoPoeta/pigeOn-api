using System.Data;

namespace pigeon_api.Messaging.Nats;

public static class Subjects
{
    public const string FriendshipRequested = "friendship.requested";
    public const string FriendshipAccepted = "friendship.accepted";
    public const string MessageCreated = "message.created";
}
