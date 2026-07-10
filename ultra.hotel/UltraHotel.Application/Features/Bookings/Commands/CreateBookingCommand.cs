using MediatR;
using UltraHotel.Application.Features.Bookings.Dtos;

namespace UltraHotel.Application.Features.Bookings.Commands;

/// <summary>Crea una reserva para una habitación.</summary>
public record CreateBookingCommand(
    Guid RoomId,
    DateOnly CheckIn,
    DateOnly CheckOut,
    string EmergencyContactName,
    string EmergencyContactPhone,
    IReadOnlyList<GuestRequest> Guests) : IRequest<BookingDto>;
