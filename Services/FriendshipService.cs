using pigeon_api.Contexts;
using Microsoft.EntityFrameworkCore;
using pigeon_api.Models;
using pigeon_api.Dtos;
using pigeon_api.Messaging.Nats;

namespace pigeon_api.Services
{
    public class FriendshipService
    {
        private readonly AppDbContext _context;
        private readonly NatsPublisher _publisher;

        public FriendshipService(AppDbContext context, NatsPublisher publisher)
        {
            _context = context;
            _publisher = publisher;
        }

        public async Task<List<Friendship>> GetUserFriendships(int userId)
        {
            return await _context.Friendships
                .Where(f => f.UserId == userId || f.FriendId == userId)
                .ToListAsync();
        }

        public async Task<Friendship?> GetFriendship(int id)
        {
            return await _context.Friendships.FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<bool> IsFrendshipMutual(int userId, int friendId)
        {
            var friendship1 = await _context.Friendships
                .FirstOrDefaultAsync(f => f.UserId == userId && f.FriendId == friendId);
            var friendship2 = await _context.Friendships
                .FirstOrDefaultAsync(f => f.UserId == friendId && f.FriendId == userId);

            return friendship1 != null && friendship2 != null;
        }

        public async Task RequestFriendship(FriendshipDto friendship)
        {
            var alreadyFriendsOrNot = await _context.Friendships.FirstOrDefaultAsync(f => f.UserId == friendship.UserId && f.FriendId == friendship.FriendId);

            if (alreadyFriendsOrNot != null)
            {
                throw new ConflictException("Friendship already exists");
            }

            var newFriendship = new Friendship
            {
                UserId = friendship.UserId,
                FriendId = friendship.FriendId,
                CreatedAt = DateTime.UtcNow,
            };

            _context.Friendships.Add(newFriendship);
            await _context.SaveChangesAsync();

            _publisher.Publish(
                Subjects.FriendshipRequested,
                new
                {
                    newFriendship.Id,
                    newFriendship.UserId,
                    newFriendship.FriendId,
                    newFriendship.CreatedAt
                }
            );
        }

        public async Task AcceptFriendship(FriendshipDto friendship)
        {
            var newFriendship = new Friendship
            {
                UserId = friendship.UserId,
                FriendId = friendship.FriendId,
                CreatedAt = DateTime.UtcNow,
            };

            _context.Friendships.Add(newFriendship);

            var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.UserId == friendship.UserId && n.FromUserId == friendship.FriendId);

            if (notification != null)
            {
                _context.Notifications.Remove(notification);
            }

            await _context.SaveChangesAsync();

            _publisher.Publish(
                Subjects.FriendshipAccepted,
                new
                {
                    newFriendship.Id,
                    newFriendship.UserId,
                    newFriendship.FriendId,
                    newFriendship.CreatedAt
                }
            );
        }

        public async Task Update(FriendshipDto friendship)
        {
            var isUserPremium = await _context.Premium
                .AnyAsync(p => p.UserId == friendship.UserId && p.ExpiryDate > DateTime.UtcNow);

            if (!isUserPremium)
            {
                throw new UnauthorizedException("Only premium users can update save method settings");
            }

            var existingFriendship = await GetFriendship(friendship.Id) ?? throw new NotFoundException("Friendship not found");
            existingFriendship.SaveOnCache = friendship.SaveOnCache;
            _context.Friendships.Update(existingFriendship);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int userId, int friendId)
        {
            var friendship = await _context.Friendships
                .Where(f => f.UserId == userId && f.FriendId == friendId)
                .ToListAsync();

            if (friendship.Count == 0)
            {
                throw new NotFoundException("Friendship not found");
            }

            foreach (var f in friendship)
            {
                _context.Friendships.Remove(f);
            }

            await _context.SaveChangesAsync();
        }
    }
}