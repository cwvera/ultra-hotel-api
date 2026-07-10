using Moq;
using UltraHotel.Application.Features.Bookings.Events;
using UltraHotel.Commons.Contracts;
using UltraHotel.Commons.Messaging;

namespace UltraHotel.Tests.Unit.Features.Bookings;

public class BookingConfirmedEventHandlerTests
{
    private readonly Mock<IMessagePublisher> _publisher = new();
    private readonly BookingConfirmedEventHandler _sut;

    public BookingConfirmedEventHandlerTests()
    {
        _sut = new BookingConfirmedEventHandler(_publisher.Object);
    }

    [Fact]
    public async Task Handle_ValidEvent_PublishesToQueue()
    {
        BookingConfirmedEvent notification = new(
            BookingId: Guid.NewGuid(),
            GuestEmail: "guest@test.com",
            GuestName: "Juan Pérez",
            HotelName: "Hotel Test",
            RoomType: "Double",
            CheckIn: new DateOnly(2025, 6, 1),
            CheckOut: new DateOnly(2025, 6, 5),
            TotalPrice: 400m);

        _publisher.Setup(p => p.PublishAsync("booking.confirmed", It.IsAny<BookingConfirmedMessage>(), It.IsAny<CancellationToken>()))
                  .Returns(Task.CompletedTask);

        await _sut.Handle(notification, CancellationToken.None);

        _publisher.Verify(p => p.PublishAsync(
            "booking.confirmed",
            It.Is<BookingConfirmedMessage>(m => m.GuestEmail == "guest@test.com" && m.TotalPrice == 400m),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
