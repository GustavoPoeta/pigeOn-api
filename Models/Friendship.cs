using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace pigeon_api.Models
{
    public class Friendship
    {
        public Friendship (int id = 0, int userId = 0, int friendId = 0, DateTime createdAt = default)
        {
            Id = id;
            UserId = userId;
            FriendId = friendId;
            CreatedAt = createdAt;
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        [Required]
        public int UserId { get; set; }

        [Column("friend_id")]
        [Required]
        public int FriendId { get; set; }

        [Column("saveoncache")]
        public bool SaveOnCache { get; set; } 

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        [ForeignKey(nameof(FriendId))]
        public User? Friend { get; set; }
    }

}