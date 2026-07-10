using Mapster;
using MediatR;
using UltraHotel.Application.Features.Bookings.Contracts;
using UltraHotel.Application.Features.Bookings.Dtos;
using UltraHotel.Application.Features.Bookings.Events;
using UltraHotel.Application.Features.Hotels.Contracts;
using UltraHotel.Application.Features.Rooms.Contracts;
using UltraHotel.Domain.Entities.Bookings;
using UltraHotel.Domain.Entities.Hotels;

namespace UltraHotel.Application.Features.Bookings.Commands;

public class CreateBookingCommandHandler(
    IRoomRepository roomRepository,
    IHotelRepository hotelRepository,
    IBookingRepository bookingRepository,
    IPublisher publisher)
    : IRequestHandler<CreateBookingCommand, BookingDto>
{
    /// <inheritdoc />
    public async Task<BookingDto> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        Room room = await roomRepository.GetByIdAsync(request.RoomId, cancellationToken)
            ?? throw new KeyNotFoundException($"Habitación {request.RoomId} no encontrada.");

        if (!room.IsEnabled)
        {
            throw new InvalidOperationException("La habitación no está disponible.");
        }

        Hotel hotel = await hotelRepository.GetByIdAsync(room.HotelId, cancellationToken)
            ?? throw new KeyNotFoundException($"Hotel {room.HotelId} no encontrado.");

        if (!hotel.IsEnabled)
        {
            throw new InvalidOperationException("El hotel no está habilitado.");
        }

        bool isAvailable = await bookingRepository.IsRoomAvailableAsync(
            room.Id, request.CheckIn, request.CheckOut, cancellationToken);

        if (!isAvailable)
        {
            throw new InvalidOperationException("La habitación no está disponible en las fechas seleccionadas.");
        }

        int nights = request.CheckOut.DayNumber - request.CheckIn.DayNumber;
        decimal totalPrice = Math.Round(room.BasePrice * (1 + room.TaxRate) * nights, 2);

        Booking booking = new()
        {
            RoomId = room.Id,
            HotelId = hotel.Id,
            CheckInDate = request.CheckIn,
            CheckOutDate = request.CheckOut,
            TotalPrice = totalPrice,
            EmergencyContactName = request.EmergencyContactName,
            EmergencyContactPhone = request.EmergencyContactPhone,
            Status = BookingStatus.Confirmed,
            Guests = request.Guests.Adapt<List<Guest>>()
        };

        await bookingRepository.AddAsync(booking, cancellationToken);

        Guest primaryGuest = booking.Guests[0];
        await publisher.Publish(new BookingConfirmedEvent(
            booking.Id,
            primaryGuest.Email,
            $"{primaryGuest.FirstName} {primaryGuest.LastName}",
            hotel.Name,
            room.RoomType.ToString(),
            booking.CheckInDate,
            booking.CheckOutDate,
            booking.TotalPrice), cancellationToken);

        return booking.Adapt<BookingDto>();
    }
}
