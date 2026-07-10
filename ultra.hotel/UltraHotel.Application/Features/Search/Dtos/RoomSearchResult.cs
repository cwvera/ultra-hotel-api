namespace UltraHotel.Application.Features.Search.Dtos;

/// <summary>Resultado de búsqueda de habitaciones disponibles.</summary>
public record RoomSearchResult(
    Guid RoomId,
    Guid HotelId,
    string HotelName,
    string City,
    string RoomType,
    int Capacity,
    decimal BasePrice,
    decimal TaxRate,
    decimal TotalPrice,
    string? LocationInHotel);
