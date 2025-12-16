using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using pigeon_api.Enums;

namespace pigeon_api.Dtos;

public sealed class NotificationDto
{
    public NotificationDto()
    {

    }

    public int Id { get; set; }
    public int UserId { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public NotificationTypes Type { get; set; }
    public string Content { get; set; } = String.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public int? FromUserId { get; set; }
}