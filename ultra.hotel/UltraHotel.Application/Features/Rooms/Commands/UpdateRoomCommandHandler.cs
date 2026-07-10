using Mapster;
using MediatR;
using UltraHotel.Application.Features.Hotels.Contracts;
using UltraHotel.Application.Features.Rooms.Contracts;
using UltraHotel.Application.Features.Rooms.Dtos;
using UltraHotel.Application.Features.Search.Contracts;
using UltraHotel.Application.Mappings;
using UltraHotel.Domain.Entities.Hotels;

namespace UltraHotel.Application.Features.Rooms.Commands;

public class UpdateRoomCommandHandler(
    IHotelRepository hotelRepository,
    IRoomRepository roomRepository,
    IElasticsearchService elasticsearchService)
    : IRequestHandler<UpdateRoomCommand, RoomDto>
{
    /// <inheritdoc />
    public async Task<RoomDto> Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
    {
        Room room = await roomRepository.GetByIdAsync(request.RoomId, cancellationToken)
            ?? throw new KeyNotFoundException($"Habitación {request.RoomId} no encontrada.");

        Hotel hotel = await hotelRepository.GetByIdAsync(room.HotelId, cancellationToken)
            ?? throw new KeyNotFoundException($"Hotel {room.HotelId} no encontrado.");

        request.Adapt(room);
        room.UpdatedAt = DateTime.UtcNow;
        await roomRepository.UpdateAsync(room, cancellationToken);

        await elasticsearchService.IndexRoomAsync(room.ToIndexDocument(hotel), cancellationToken);

        return room.Adapt<RoomDto>();
    }
}
