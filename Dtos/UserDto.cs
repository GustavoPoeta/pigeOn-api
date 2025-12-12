namespace pigeon_api.Dtos
{
    public class UserDto
    {
        public UserDto(int id, string username, string email, string password, string? photoPath = null)
        {
            Id = id;
            Username = username;
            Email = email;
            Password = password;
            PhotoPath = photoPath;
        }
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? PhotoPath { get; set; }
    }
}