using MediatR;
using UltraHotel.Commons.Contracts;
using UltraHotel.Commons.Messaging;

namespace UltraHotel.Application.Features.Bookings.Events;

public class BookingConfirmedEventHandler(IMessagePublisher messagePublisher)
    : INotificationHandler<BookingConfirmedEvent>
{
    private const string Queue = "booking.confirmed";

    /// <inheritdoc />
    public Task Handle(BookingConfirmedEvent notification, CancellationToken cancellationToken) =>
        messagePublisher.PublishAsync(Queue, new BookingConfirmedMessage(
            notification.BookingId,
            notification.GuestEmail,
            notification.GuestName,
            notification.HotelName,
            notification.RoomType,
            notification.CheckIn,
            notification.CheckOut,
            notification.TotalPrice), cancellationToken);
}
