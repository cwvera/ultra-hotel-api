using MediatR;

namespace UltraHotel.Application.Features.Bookings.Events;

public record BookingConfirmedEvent(
    Guid     BookingId,
    string   GuestEmail,
    string   GuestName,
    string   HotelName,
    string   RoomType,
    DateOnly CheckIn,
    DateOnly CheckOut,
    decimal  TotalPrice) : INotification;
