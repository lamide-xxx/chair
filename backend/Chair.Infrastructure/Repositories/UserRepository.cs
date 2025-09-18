using Chair.Domain.Entities;
using Chair.Domain.Repositories;
using Chair.Infrastructure.Persistence;

namespace Chair.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<User> AddUserAsync(User user)
    {
        user.Id = Guid.NewGuid();
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }
}