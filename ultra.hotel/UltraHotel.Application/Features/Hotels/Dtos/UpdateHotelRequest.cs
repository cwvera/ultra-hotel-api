namespace UltraHotel.Application.Features.Hotels.Dtos;

/// <summary>Datos del cuerpo para actualizar un hotel (sin incluir el ID de ruta).</summary>
public record UpdateHotelRequest(
    string Name,
    string City,
    string Address,
    string? Description);
