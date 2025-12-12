using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pigeon_api.Models
{
    public class Premium
    {
        public Premium() {}

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("last_renovation_at")]
        public DateTime LastRenovationAt { get; set; }
        [Column("expiry_date")]
        public DateTime ExpiryDate { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
    }
}