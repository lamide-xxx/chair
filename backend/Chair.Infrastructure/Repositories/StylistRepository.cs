using Chair.Domain.Entities;
using Chair.Domain.Repositories;
using Chair.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Chair.Infrastructure.Repositories;

public class StylistRepository : IStylistRepository
{
    private readonly AppDbContext _context;
    
    public StylistRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Stylist> AddStylistAsync(Stylist stylist)
    {
        stylist.Id = Guid.NewGuid();
        await _context.Stylists.AddAsync(stylist);
        await _context.SaveChangesAsync();
        return stylist;
    }

    public async Task<IEnumerable<Stylist>> GetAllStylistsAsync()
    {
        return await _context.Stylists.ToListAsync();
    }
    
    public async Task<Stylist?> GetStylistByIdAsync(Guid id)
    {
        return await _context.Stylists.FindAsync(id);
    }
}