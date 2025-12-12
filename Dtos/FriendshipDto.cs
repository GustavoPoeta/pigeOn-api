namespace pigeon_api.Dtos
{
    public class FriendshipDto
    {
        public FriendshipDto()
        {
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public int FriendId { get; set; }
        public bool SaveOnCache { get; set;}
        public DateTime CreatedAt { get; set; }
    }
}