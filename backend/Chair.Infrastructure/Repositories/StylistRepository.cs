using Chair.Domain.Entities;
using Chair.Domain.Repositories;

namespace Chair.Infrastructure.Repositories;

public class StylistRepository : IStylistRepository
{
    private readonly List<Stylist> _stylists = new();
    
    public Stylist AddStylist(Stylist stylist)
    {
        _stylists.Add(stylist);
        return stylist;
    }

    public IEnumerable<Stylist> GetAllStylists()
    {
        return _stylists;
    }
    
    public Stylist? GetStylistById(Guid id)
    {
        return _stylists.FirstOrDefault(s => s.Id == id);
    }
}