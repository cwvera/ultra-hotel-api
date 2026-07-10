using Mapster;
using MediatR;
using UltraHotel.Application.Features.Hotels.Contracts;
using UltraHotel.Application.Features.Rooms.Contracts;
using UltraHotel.Application.Features.Rooms.Dtos;
using UltraHotel.Application.Features.Search.Contracts;
using UltraHotel.Application.Features.Search.Dtos;
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

        return room.Adapt<RoomDto>();
    }
}
