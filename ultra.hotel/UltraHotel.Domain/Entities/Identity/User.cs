using UltraHotel.Domain.Common;

namespace UltraHotel.Domain.Entities.Identity;

public class User : BaseEntity
{
    public string   Email        { get; set; } = string.Empty;
    public string   PasswordHash { get; set; } = string.Empty;
    public Role     Role         { get; set; }
    public DateTime CreatedAt    { get; set; } = DateTime.UtcNow;
}
