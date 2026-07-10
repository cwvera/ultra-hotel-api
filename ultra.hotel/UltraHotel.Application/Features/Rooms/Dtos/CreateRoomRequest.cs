using UltraHotel.Domain.Entities.Hotels;

namespace UltraHotel.Application.Features.Rooms.Dtos;

/// <summary>Datos del cuerpo para agregar una habitación (el hotel viene del parámetro de ruta).</summary>
public record CreateRoomRequest(
    RoomType RoomType,
    decimal BasePrice,
    decimal TaxRate,
    int Capacity,
    string? LocationInHotel);
