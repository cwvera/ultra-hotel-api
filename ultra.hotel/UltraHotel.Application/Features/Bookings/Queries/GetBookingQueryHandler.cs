using Mapster;
using MediatR;
using UltraHotel.Application.Features.Bookings.Contracts;
using UltraHotel.Application.Features.Bookings.Dtos;
using UltraHotel.Domain.Entities.Bookings;

namespace UltraHotel.Application.Features.Bookings.Queries;

public class GetBookingQueryHandler(IBookingRepository bookingRepository)
    : IRequestHandler<GetBookingQuery, BookingDto>
{
    /// <inheritdoc />
    public async Task<BookingDto> Handle(GetBookingQuery request, CancellationToken cancellationToken)
    {
        Booking booking = await bookingRepository.GetByIdAsync(request.BookingId, cancellationToken)
            ?? throw new KeyNotFoundException($"Reserva {request.BookingId} no encontrada.");
        return booking.Adapt<BookingDto>();
    }
}
