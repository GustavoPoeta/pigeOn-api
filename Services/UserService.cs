using Microsoft.EntityFrameworkCore;
using pigeon_api.Contexts;
using pigeon_api.Models;

public class UserService(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public async Task<List<User>> GetAll()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User> Get(int? id = null)
    {
        return await _context.Users.FirstAsync(user => user.Id == id);
    }
}