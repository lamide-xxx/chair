namespace Chair.Api.DTOs;

public class ServiceDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
}