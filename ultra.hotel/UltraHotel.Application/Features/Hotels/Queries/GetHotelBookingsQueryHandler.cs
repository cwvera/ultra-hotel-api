using Mapster;
using MediatR;
using UltraHotel.Application.Features.Bookings.Contracts;
using UltraHotel.Application.Features.Hotels.Contracts;
using UltraHotel.Application.Features.Hotels.Dtos;
using UltraHotel.Domain.Entities.Bookings;

namespace UltraHotel.Application.Features.Hotels.Queries;

public class GetHotelBookingsQueryHandler(
    IHotelRepository hotelRepository,
    IBookingRepository bookingRepository)
    : IRequestHandler<GetHotelBookingsQuery, IReadOnlyList<HotelBookingSummaryDto>>
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<HotelBookingSummaryDto>> Handle(
        GetHotelBookingsQuery request, CancellationToken cancellationToken)
    {
        if (!await hotelRepository.ExistsByIdAsync(request.HotelId, cancellationToken))
        {
            throw new KeyNotFoundException($"Hotel {request.HotelId} no encontrado.");
        }

        IReadOnlyList<Booking> bookings = await bookingRepository.GetByHotelIdAsync(request.HotelId, cancellationToken);
        return bookings.Adapt<IReadOnlyList<HotelBookingSummaryDto>>();
    }
}
