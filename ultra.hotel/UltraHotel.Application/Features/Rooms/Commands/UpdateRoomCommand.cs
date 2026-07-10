using MediatR;
using UltraHotel.Application.Features.Rooms.Dtos;
using UltraHotel.Domain.Entities.Hotels;

namespace UltraHotel.Application.Features.Rooms.Commands;

/// <summary>Actualiza los datos de una habitación existente.</summary>
public record UpdateRoomCommand(
    Guid RoomId,
    RoomType RoomType,
    decimal BasePrice,
    decimal TaxRate,
    int Capacity,
    string? LocationInHotel) : IRequest<RoomDto>;
