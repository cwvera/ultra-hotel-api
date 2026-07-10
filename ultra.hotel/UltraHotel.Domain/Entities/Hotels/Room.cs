using UltraHotel.Domain.Common;

namespace UltraHotel.Domain.Entities.Hotels;

public class Room : AuditableEntity
{
    public Guid     HotelId         { get; set; }
    public RoomType RoomType        { get; set; }
    public decimal  BasePrice       { get; set; }
    public decimal  TaxRate         { get; set; }
    public int      Capacity        { get; set; }
    public string?  LocationInHotel { get; set; }
    public bool     IsEnabled       { get; set; } = true;

    public decimal TotalPrice => Math.Round(BasePrice * (1 + TaxRate), 2);
}
