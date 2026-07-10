namespace UltraHotel.Application.Features.Hotels.Dtos;

public record HotelDto(
    Guid Id,
    string Name,
    string City,
    string Address,
    string? Description,
    string AgentEmail,
    bool IsEnabled,
    DateTime CreatedAt);
