using UltraHotel.Domain.Common;

namespace UltraHotel.Domain.Entities.Hotels;

public class Hotel : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string AgentEmail { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
}
