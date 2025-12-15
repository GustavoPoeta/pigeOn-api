using Microsoft.EntityFrameworkCore;
using pigeon_api.Contexts;
using pigeon_api.Dtos;
using pigeon_api.Models;

namespace pigeon_api.Services;

public class MessageService
{
    private readonly AppDbContext _context;

    public MessageService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<MessageDto>> GetUserMessages(int userId)
    {
        var messages = await _context.Messages
            .Where(m => m.SenderId == userId && m.Deleted != true)
            .OrderBy(m => m.CreatedAt)
            .Select(m => new MessageDto(
                m.Id,
                m.SenderId,
                m.ReceiverId,
                m.Content,
                m.Edited,
                m.Deleted,
                m.CreatedAt,
                m.ViewedAt))
            .ToListAsync();
        
        if (messages.Count == 0)
        {
            throw new NotFoundException("No messages found for the specified user.");
        }

        return messages;
    }

    public async Task<List<MessageDto>> GetFriendshipMessages(int userId, int friendId)
    {
        var messages = await _context.Messages
            .Where(m => (m.SenderId == userId && m.ReceiverId == friendId) ||
                        (m.SenderId == friendId && m.ReceiverId == userId) && m.Deleted != true)
            .OrderBy(m => m.CreatedAt)
            .Select(m => new MessageDto(
                m.Id,
                m.SenderId,
                m.ReceiverId,
                m.Content,
                m.Edited,
                m.Deleted,
                m.CreatedAt,
                m.ViewedAt))
            .ToListAsync();

        if (messages.Count == 0)
        {
            throw new NotFoundException("No messages found between the specified users.");
        }

        return messages;
    }

    public async Task CreateMessage(MessageDto messageDto)
    {
        var newMessage = new Message(
            0,
            messageDto.SenderId,
            messageDto.ReceiverId,
            messageDto.Content,
            false,
            false,
            DateTime.UtcNow,
            null
        );

        _context.Messages.Add(newMessage);
        await _context.SaveChangesAsync();
    }

    public async Task MarkMessageAsViewed(int messageId)
    {
        var message = await _context.Messages.FindAsync(messageId);
        if (message == null)
        {
            throw new NotFoundException("Message not found.");
        }

        message.ViewedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task UpdateMessageContent (UpdateMessageDto updateMessageDto)
    {
        var message = await _context.Messages.FindAsync(updateMessageDto.MessageId);
        if (message == null)
        {
            throw new NotFoundException("Message not found.");
        }

        message.Content = updateMessageDto.Content;
        message.Edited = true;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteMessage (int messageId)
    {
        var message = await _context.Messages.FindAsync(messageId);
        if (message == null)
        {
            throw new NotFoundException("Message not found.");
        }

        message.Deleted = true;
        await _context.SaveChangesAsync();
    }
}