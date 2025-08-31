using Chair.Domain.Entities;

namespace Chair.Domain.Repositories;

public interface IStylistRepository
{
    Stylist AddStylist(Stylist stylist);
    IEnumerable<Stylist> GetAllStylists();
    Stylist? GetStylistById(Guid id);
}