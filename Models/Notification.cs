using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using pigeon_api.Enums;

namespace pigeon_api.Models;

public sealed class Notification
{
    public Notification ()
    {
        
    }

    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Column("user_id")]
    public int UserId { get; set; }
    [Column("type")]
    public NotificationTypes Type { get; set; }
    [Column("content")]
    public string Content { get; set; } = String.Empty;
    [Column("is_read")]
    public bool IsRead { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    [Column("read_at")]
    public DateTime? ReadAt { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }
}