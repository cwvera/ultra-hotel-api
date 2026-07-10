using MediatR;
using UltraHotel.Application.Features.Bookings.Dtos;

namespace UltraHotel.Application.Features.Bookings.Queries;

/// <summary>Obtiene el detalle de una reserva.</summary>
public record GetBookingQuery(Guid BookingId) : IRequest<BookingDto>;
