using Chair.Domain.Entities;

namespace Chair.Domain.Repositories;

public interface IServiceRepository
{
    Service? GetServiceById(Guid id);
    IEnumerable<Service> GetAllServices();
    Service AddService(Service service);
}