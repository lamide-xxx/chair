using Chair.Domain.Entities;

namespace Chair.Domain.Repositories;

public interface IUserRepository
{
    Task<User> AddUserAsync(User user);
    Task<User?> GetUserByIdAsync(Guid id);
}