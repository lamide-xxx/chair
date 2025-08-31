namespace Chair.Domain.Entities;

public class Stylist
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FullName { get; set; } = null!;
    public List<string> Specialties { get; set; } = new();
    public double Rating { get; set; } = 0.0;
    public string Location { get; set; } = null!;
    public List<Guid> ServiceIds { get; set; } = new();
}