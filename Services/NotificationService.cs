using Microsoft.EntityFrameworkCore;
using pigeon_api.Contexts;
using pigeon_api.Dtos;
using pigeon_api.Models;

namespace pigeon_api.Services;

public class NotificationService
{
    private readonly AppDbContext _context;

    public NotificationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<NotificationDto>> GetUnreadNotifications(int userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && n.IsRead == false)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                UserId = n.UserId,
                Type = n.Type,
                Content = n.Content,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
                ReadAt = n.ReadAt
            })
            .ToListAsync();

        if (notifications.Count == 0)
        {
            throw new NotFoundException("No unread notifications found for the specified user.");
        }

        return notifications;
    }

    public async Task CreateNotification(NotificationDto notificationDto)
    {
        var newNotification = new Notification
        {
            UserId = notificationDto.UserId,
            Type = notificationDto.Type,
            Content = notificationDto.Content,
            IsRead = notificationDto.IsRead,
            CreatedAt = notificationDto.CreatedAt,
            ReadAt = notificationDto.ReadAt
        };
        await _context.Notifications.AddAsync(newNotification);
        await _context.SaveChangesAsync();
    }

    public async Task MarkNotificationAsRead (int notificationId)
    {
        var notification = await _context.Notifications.FindAsync(notificationId);
        if (notification == null)
        {
            throw new NotFoundException("Notification not found.");
        }

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;
        _context.Update(notification);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteNotification (int notificationId)
    {
        var notification = await _context.Notifications.FindAsync(notificationId);
        if (notification == null)
        {
            throw new NotFoundException("Notification not found.");
        }
        _context.Notifications.Remove(notification);
        await _context.SaveChangesAsync();
    }
}