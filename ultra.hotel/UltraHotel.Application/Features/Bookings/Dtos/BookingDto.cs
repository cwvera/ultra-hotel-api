namespace UltraHotel.Application.Features.Bookings.Dtos;

public record BookingDto(
    Guid Id,
    Guid RoomId,
    Guid HotelId,
    DateOnly CheckIn,
    DateOnly CheckOut,
    string Status,
    decimal TotalPrice,
    string EmergencyContactName,
    string EmergencyContactPhone,
    IReadOnlyList<GuestDto> Guests,
    DateTime CreatedAt);
