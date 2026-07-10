using MediatR;
using UltraHotel.Application.Features.Rooms.Dtos;
using UltraHotel.Domain.Entities.Hotels;

namespace UltraHotel.Application.Features.Rooms.Commands;

/// <summary>Agrega una habitación a un hotel existente.</summary>
public record CreateRoomCommand(
    Guid HotelId,
    RoomType RoomType,
    decimal BasePrice,
    decimal TaxRate,
    int Capacity,
    string? LocationInHotel) : IRequest<RoomDto>;
