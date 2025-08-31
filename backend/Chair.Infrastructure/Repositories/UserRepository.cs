using Chair.Domain.Entities;
using Chair.Domain.Repositories;

namespace Chair.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly List<User> _users = new();
    
    public User AddUser(User user)
    {
        _users.Add(user);
        return user;
    }

    public User? GetUserById(Guid id)
    {
        return _users.FirstOrDefault(u => u.Id == id);
    }
}