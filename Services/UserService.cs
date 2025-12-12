using Microsoft.EntityFrameworkCore;
using pigeon_api.Contexts;
using pigeon_api.Models;
using pigeon_api.Dtos;

public class UserService(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public async Task<List<User>> GetAll()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> Get(int? id = null)
    {
        return await _context.Users.FirstAsync(user => user.Id == id);
    }

    public async Task Create(UserDto user)
    {
        var newUser = new User 
        {
            Username = user.Username,
            Email = user.Email,
            Password = user.Password,
            PhotoPath = user.PhotoPath
        };
        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();
    }
    
    public async Task Update (UserDto user)
    {
        var updatedUser = new User
        (
            user.Id,
            user.Username,
            user.Email,
            user.Password,
            user.PhotoPath
        );

        _context.Users.Update(updatedUser);
        await _context.SaveChangesAsync();
    }

    public async Task Delete (int id)
    {
        var user = await Get(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        } else
        {
            throw new Exception("User not found");
        }
    }
}