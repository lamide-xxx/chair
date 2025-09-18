using Chair.Domain.Entities;
using Chair.Domain.Repositories;
using Chair.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Chair.Infrastructure.Repositories;

public class ServiceRepository : IServiceRepository
{
    private readonly AppDbContext _context;
    
    public ServiceRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Service> AddServiceAsync(Service service)
    {
        service.Id = Guid.NewGuid();
        await _context.Services.AddAsync(service);
        await _context.SaveChangesAsync();
        return service;
    }
    
    public async Task<IEnumerable<Service>> GetAllServicesAsync()
    {
        return await _context.Services.ToListAsync();
    }
    
    public async Task<Service?> GetServiceByIdAsync(Guid id)
    {
        return await _context.Services.FindAsync(id);
    }
}