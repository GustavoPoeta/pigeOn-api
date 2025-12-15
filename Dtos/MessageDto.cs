namespace pigeon_api.Dtos;

public sealed class MessageDto
{

    public MessageDto (int id, int senderId, int receiverId, string content, Boolean edited, Boolean deleted, DateTime createdAt, DateTime? viewedAt)
    {
        Id = id;
        SenderId = senderId;
        ReceiverId = receiverId;
        Content = content;
        Edited = edited;
        Deleted = deleted;
        CreatedAt = createdAt;
        ViewedAt = viewedAt;
    }

    public int Id { get; set; }
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    public string Content { get; set; }
    public Boolean Edited { get; set; }
    public Boolean Deleted { get; set; }
    public DateTime CreatedAt { get; set; } 
    public DateTime? ViewedAt { get; set; }
}