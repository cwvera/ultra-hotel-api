using Mapster;
using MediatR;
using UltraHotel.Application.Features.Hotels.Contracts;
using UltraHotel.Application.Features.Hotels.Dtos;
using UltraHotel.Domain.Entities.Hotels;

namespace UltraHotel.Application.Features.Hotels.Commands;

public class UpdateHotelCommandHandler(IHotelRepository hotelRepository)
    : IRequestHandler<UpdateHotelCommand, HotelDto>
{
    /// <inheritdoc />
    public async Task<HotelDto> Handle(UpdateHotelCommand request, CancellationToken cancellationToken)
    {
        Hotel hotel = await hotelRepository.GetByIdAsync(request.HotelId, cancellationToken)
            ?? throw new KeyNotFoundException($"Hotel {request.HotelId} no encontrado.");

        request.Adapt(hotel);
        hotel.UpdatedAt = DateTime.UtcNow;
        await hotelRepository.UpdateAsync(hotel, cancellationToken);
        return hotel.Adapt<HotelDto>();
    }
}
