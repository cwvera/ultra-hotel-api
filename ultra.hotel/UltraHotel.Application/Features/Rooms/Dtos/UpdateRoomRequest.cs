using UltraHotel.Domain.Entities.Hotels;

namespace UltraHotel.Application.Features.Rooms.Dtos;

/// <summary>Datos del cuerpo para actualizar una habitación (el ID viene del parámetro de ruta).</summary>
public record UpdateRoomRequest(
    RoomType RoomType,
    decimal BasePrice,
    decimal TaxRate,
    int Capacity,
    string? LocationInHotel);
