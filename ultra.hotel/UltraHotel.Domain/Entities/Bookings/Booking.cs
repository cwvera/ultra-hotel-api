using UltraHotel.Domain.Common;

namespace UltraHotel.Domain.Entities.Bookings;

public class Booking : BaseEntity
{
    public Guid RoomId { get; set; }
    public Guid HotelId { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public BookingStatus Status { get; set; }
    public decimal TotalPrice { get; set; }
    public string EmergencyContactName { get; set; } = string.Empty;
    public string EmergencyContactPhone { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<Guest> Guests { get; set; } = [];

    public int NumberOfNights => CheckOutDate.DayNumber - CheckInDate.DayNumber;
}
