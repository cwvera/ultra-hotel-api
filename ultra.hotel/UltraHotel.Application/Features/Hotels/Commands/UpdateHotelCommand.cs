using MediatR;
using UltraHotel.Application.Features.Hotels.Dtos;

namespace UltraHotel.Application.Features.Hotels.Commands;

/// <summary>Actualiza los datos de un hotel existente.</summary>
public record UpdateHotelCommand(
    Guid HotelId,
    string Name,
    string City,
    string Address,
    string? Description) : IRequest<HotelDto>;
