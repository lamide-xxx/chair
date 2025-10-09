using Chair.Domain.Entities;

namespace Chair.Api.DTOs.Ai;

public class AiStylistRequest
{
    public required string Preference { get; set;  }
    public required List<Stylist> Stylists { get; set; }
}