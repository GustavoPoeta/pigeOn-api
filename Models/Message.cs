using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pigeon_api.Models;

public class Message
{
    public Message (int id, int senderId, int receiverId, string content, Boolean edited, Boolean deleted, DateTime createdAt, DateTime? viewedAt)
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

    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Column("sender_id")]
    public int SenderId { get; set; }
    [Column("receiver_id")]
    public int ReceiverId { get; set; }
    [Column("message")]
    [MaxLength(2500)]
    [Required]
    public string Content { get; set; }
    [Column("edited")]
    public Boolean Edited { get; set; }
    [Column("deleted")]
    public Boolean Deleted { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    [Column("viewed_at")]
    public DateTime? ViewedAt { get; set; }

    [ForeignKey(nameof(SenderId))]
    public User? Sender { get; set; }
    [ForeignKey(nameof(ReceiverId))]
    public User? Receiver { get; set; }
}