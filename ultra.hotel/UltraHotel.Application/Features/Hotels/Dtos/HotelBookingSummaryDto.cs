namespace UltraHotel.Application.Features.Hotels.Dtos;

public record HotelBookingSummaryDto(
    Guid BookingId,
    Guid RoomId,
    DateOnly CheckIn,
    DateOnly CheckOut,
    string Status,
    decimal TotalPrice,
    int GuestCount,
    DateTime CreatedAt);
