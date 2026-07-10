using MediatR;
using UltraHotel.Application.Features.Hotels.Contracts;
using UltraHotel.Application.Features.Rooms.Contracts;
using UltraHotel.Application.Features.Search.Contracts;
using UltraHotel.Application.Features.Search.Dtos;
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

        await elasticsearchService.IndexRoomAsync(new RoomIndexDocument
        {
            RoomId = room.Id.ToString(),
            HotelId = hotel.Id.ToString(),
            HotelName = hotel.Name,
            City = hotel.City.ToLower(),
            RoomType = room.RoomType.ToString().ToUpper(),
            Capacity = room.Capacity,
            BasePrice = (double)room.BasePrice,
            TaxRate = (double)room.TaxRate,
            LocationInHotel = room.LocationInHotel,
            IsAvailable = true,
            HotelEnabled = hotel.IsEnabled,
            RoomEnabled = room.IsEnabled,
            YearMonth = DateTime.UtcNow.ToString("yyyyMM")
        }, cancellationToken);
    }
}
