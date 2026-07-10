namespace UltraHotel.Commons.Messaging;

public record BookingConfirmedMessage(
    Guid BookingId,
    string GuestEmail,
    string GuestName,
    string HotelName,
    string RoomType,
    DateOnly CheckIn,
    DateOnly CheckOut,
    decimal TotalPrice);
