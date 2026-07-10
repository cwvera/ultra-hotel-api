using Mapster;
using MediatR;
using UltraHotel.Application.Features.Hotels.Contracts;
using UltraHotel.Application.Features.Hotels.Dtos;
using UltraHotel.Domain.Entities.Hotels;

namespace UltraHotel.Application.Features.Hotels.Commands;

public class CreateHotelCommandHandler(IHotelRepository hotelRepository)
    : IRequestHandler<CreateHotelCommand, HotelDto>
{
    /// <inheritdoc />
    public async Task<HotelDto> Handle(CreateHotelCommand request, CancellationToken cancellationToken)
    {
        Hotel hotel = request.Adapt<Hotel>();
        await hotelRepository.AddAsync(hotel, cancellationToken);
        return hotel.Adapt<HotelDto>();
    }
}
