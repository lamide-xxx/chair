using Chair.Domain.Entities;

namespace Chair.Domain.Repositories;

public interface IStylistRepository
{
    Task<Stylist> AddStylistAsync(Stylist stylist);
    Task<IEnumerable<Stylist>> GetAllStylistsAsync();
    Task<Stylist?> GetStylistByIdAsync(Guid id);
}