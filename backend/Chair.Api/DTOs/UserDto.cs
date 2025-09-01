namespace Chair.Api.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
}