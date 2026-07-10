using MediatR;
using UltraHotel.Application.Features.Hotels.Contracts;
using UltraHotel.Application.Features.Rooms.Contracts;
using UltraHotel.Application.Features.Search.Contracts;
using UltraHotel.Application.Mappings;
using UltraHotel.Domain.Entities.Hotels;

namespace UltraHotel.Application.Features.Rooms.Commands;

public class ToggleRoomStatusCommandHandler(
    IRoomRepository roomRepository,
    IHotelRepository hotelRepository,
    IElasticsearchService elasticsearchService)
    : IRequestHandler<ToggleRoomStatusCommand>
{
    /// <inheritdoc />
    public async Task Handle(ToggleRoomStatusCommand request, CancellationToken cancellationToken)
    {
        Room room = await roomRepository.GetByIdAsync(request.RoomId, cancellationToken)
            ?? throw new KeyNotFoundException($"Habitación {request.RoomId} no encontrada.");

        Hotel hotel = await hotelRepository.GetByIdAsync(room.HotelId, cancellationToken)
            ?? throw new KeyNotFoundException($"Hotel {room.HotelId} no encontrado.");

        room.IsEnabled = request.IsEnabled;
        room.UpdatedAt = DateTime.UtcNow;
        await roomRepository.UpdateAsync(room, cancellationToken);

        await elasticsearchService.IndexRoomAsync(room.ToIndexDocument(hotel), cancellationToken);
    }
}
