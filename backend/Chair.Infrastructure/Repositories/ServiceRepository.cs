using Chair.Domain.Entities;
using Chair.Domain.Repositories;

namespace Chair.Infrastructure.Repositories;

public class ServiceRepository : IServiceRepository
{
    private readonly List<Service> _services = new();
    
    public Service AddService(Service service)
    {
        _services.Add(service);
        return service;
    }
    
    public IEnumerable<Service> GetAllServices()
    {
        return _services;
    }
    
    public Service? GetServiceById(Guid id)
    {
        return _services.FirstOrDefault(s => s.Id == id);
    }
}