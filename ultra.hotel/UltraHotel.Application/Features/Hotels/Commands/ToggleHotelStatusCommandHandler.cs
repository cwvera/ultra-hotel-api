using MediatR;
using UltraHotel.Application.Features.Hotels.Contracts;
using UltraHotel.Application.Features.Rooms.Contracts;
using UltraHotel.Application.Features.Search.Contracts;
using UltraHotel.Application.Features.Search.Dtos;
using UltraHotel.Domain.Entities.Hotels;

namespace UltraHotel.Application.Features.Hotels.Commands;

public class ToggleHotelStatusCommandHandler(
    IHotelRepository hotelRepository,
    IRoomRepository roomRepository,
    IElasticsearchService elasticsearchService)
    : IRequestHandler<ToggleHotelStatusCommand>
{
    /// <inheritdoc />
    public async Task Handle(ToggleHotelStatusCommand request, CancellationToken cancellationToken)
    {
        Hotel hotel = await hotelRepository.GetByIdAsync(request.HotelId, cancellationToken)
            ?? throw new KeyNotFoundException($"Hotel {request.HotelId} no encontrado.");

        hotel.IsEnabled = request.IsEnabled;
        hotel.UpdatedAt = DateTime.UtcNow;
        await hotelRepository.UpdateAsync(hotel, cancellationToken);

        IReadOnlyList<Room> rooms = await roomRepository.GetByHotelIdAsync(request.HotelId, cancellationToken);

        IEnumerable<Task> indexTasks = rooms.Select(room =>
            elasticsearchService.IndexRoomAsync(new RoomIndexDocument
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
            }, cancellationToken));

        await Task.WhenAll(indexTasks);
    }
}
