using Mapster;
using MediatR;
using UltraHotel.Application.Features.Hotels.Contracts;
using UltraHotel.Application.Features.Rooms.Contracts;
using UltraHotel.Application.Features.Rooms.Dtos;
using UltraHotel.Application.Features.Search.Contracts;
using UltraHotel.Application.Mappings;
using UltraHotel.Domain.Entities.Hotels;

namespace UltraHotel.Application.Features.Rooms.Commands;

public class CreateRoomCommandHandler(
    IHotelRepository hotelRepository,
    IRoomRepository roomRepository,
    IElasticsearchService elasticsearchService)
    : IRequestHandler<CreateRoomCommand, RoomDto>
{
    /// <inheritdoc />
    public async Task<RoomDto> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        Hotel hotel = await hotelRepository.GetByIdAsync(request.HotelId, cancellationToken)
            ?? throw new KeyNotFoundException($"Hotel {request.HotelId} no encontrado.");

        Room room = request.Adapt<Room>();
        await roomRepository.AddAsync(room, cancellationToken);

        await elasticsearchService.IndexRoomAsync(room.ToIndexDocument(hotel), cancellationToken);

        return room.Adapt<RoomDto>();
    }
}
