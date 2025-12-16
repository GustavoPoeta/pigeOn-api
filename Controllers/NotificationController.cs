using Microsoft.AspNetCore.Mvc;
using pigeon_api.Dtos;
using pigeon_api.Services;

namespace pigeon_api.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationController : ControllerBase
{
    private readonly NotificationService _service;

    public NotificationController(NotificationService service)
    {
        _service = service;
    }

    [HttpGet("unread/{userId:int}")]
    public async Task<IActionResult> GetUnreadNotifications(int userId)
    {
        var notifications = await _service.GetUnreadNotifications(userId);
        return Ok(notifications);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateNotification([FromBody] NotificationDto notificationDto)
    {
        await _service.CreateNotification(notificationDto);
        return Created();
    }

    [HttpPut("markAsRead/{notificationId:int}")]
    public async Task<IActionResult> MarkNotificationAsRead(int notificationId)
    {
        await _service.MarkNotificationAsRead(notificationId);
        return Ok();
    }

    [HttpDelete("delete/{notificationId:int}")]
    public async Task<IActionResult> DeleteNotification(int notificationId)
    {
        await _service.DeleteNotification(notificationId);
        return Ok();
    }
}