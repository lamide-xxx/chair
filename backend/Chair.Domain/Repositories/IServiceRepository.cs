using Chair.Domain.Entities;

namespace Chair.Domain.Repositories;

public interface IServiceRepository
{
    Task<Service?> GetServiceByIdAsync(Guid id);
    Task<IEnumerable<Service>> GetAllServicesAsync();
    Task<Service> AddServiceAsync(Service service);
}