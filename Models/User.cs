
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pigeon_api.Models
{

    public class User
    {
        public User(int id, string username, string email, string password, string? photoPath = null)
        {
            Id = id;
            Username = username;
            Email = email;
            Password = password;
            PhotoPath = photoPath;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [Column("username")]
        [MaxLength(50)]
        private string Username { get; set; }

        [Required]
        [Column("email")]
        [MaxLength(320)]
        private string Email { get; set; }

        [Required]
        [Column("password")]
        [MaxLength(255)]
        private string Password { get; set; }

        [MaxLength(255)]
        [Column("photopath")]
        private string? PhotoPath { get; set; }

        // getters
        public string GetUsername() => Username;
        public string GetEmail() => Email;
        public string GetPassword() => Password;
        public string GetPhotoPath() => PhotoPath;

        // setters
        public void SetUsername(string username) => Username = username;
        public void SetEmail(string email) => Email = email;
        public void SetPassword(string password) => Password = password;
        public void SetPhotoPath(string photoPath) => PhotoPath = photoPath;

    }
}