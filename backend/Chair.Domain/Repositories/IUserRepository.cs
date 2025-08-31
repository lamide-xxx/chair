using Chair.Domain.Entities;

namespace Chair.Domain.Repositories;

public interface IUserRepository
{
    User AddUser(User user);
    User? GetUserById(Guid id);
}