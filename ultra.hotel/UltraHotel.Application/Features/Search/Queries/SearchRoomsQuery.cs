using MediatR;
using UltraHotel.Application.Features.Search.Dtos;

namespace UltraHotel.Application.Features.Search.Queries;

/// <summary>Busca habitaciones disponibles por ciudad, fechas y número de huéspedes.</summary>
public record SearchRoomsQuery(
    string City,
    DateOnly CheckIn,
    DateOnly CheckOut,
    int Guests) : IRequest<IReadOnlyList<RoomSearchResult>>;
