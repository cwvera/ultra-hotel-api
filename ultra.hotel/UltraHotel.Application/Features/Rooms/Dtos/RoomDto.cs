namespace UltraHotel.Application.Features.Rooms.Dtos;

public record RoomDto(
    Guid Id,
    Guid HotelId,
    string RoomType,
    decimal BasePrice,
    decimal TaxRate,
    decimal TotalPrice,
    int Capacity,
    string? LocationInHotel,
    bool IsEnabled,
    DateTime CreatedAt);
