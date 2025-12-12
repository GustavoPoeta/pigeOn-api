
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pigeon_api.Models
{

    public class User
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public User() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        public User(int id, string username, string email, string password, string? photoPath = null)
        {
            Id = id;
            Username = username;
            Email = email;
            Password = password;
            PhotoPath = photoPath;
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("username")]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [Column("email")]
        [MaxLength(320)]
        public string Email { get; set; }

        [Required]
        [Column("password")]
        [MaxLength(255)]
        public string Password { get; set; }

        [MaxLength(255)]
        [Column("photopath")]
        public string? PhotoPath { get; set; }
    }
}