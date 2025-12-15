namespace pigeon_api.Dtos;

public sealed class UpdateMessageDto
{
    public int MessageId { get; init; }
    public string Content { get; init; } = string.Empty;
}
