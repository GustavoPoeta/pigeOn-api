using pigeon_api.Contexts;
using Microsoft.EntityFrameworkCore;
using pigeon_api.Models;
using pigeon_api.Dtos;

namespace pigeon_api.Services
{
    public class FriendshipService(AppDbContext context)
    {
        private readonly AppDbContext _context = context;

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

        public async Task Create(FriendshipDto friendship)
        {
            var newFriendship = new Friendship
            {
                UserId = friendship.UserId,
                FriendId = friendship.FriendId,
                CreatedAt = friendship.CreatedAt
            };

            _context.Friendships.Add(newFriendship);
            await _context.SaveChangesAsync();
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