namespace pigeon_api.Dtos;

public sealed class PremiumDto
{
    public PremiumDto() { }

    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastRenovationAt { get; set; }
    public DateTime ExpiryDate { get; set; }
}
