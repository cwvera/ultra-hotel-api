using Microsoft.Extensions.Logging.Abstractions;
using UltraHotel.Commons.Messaging;
using UltraHotel.Infrastructure.Messaging;

namespace UltraHotel.Tests.Unit.Infrastructure.Messaging;

public class LogEmailServiceTests
{
    private readonly LogEmailService _sut = new(NullLogger<LogEmailService>.Instance);

    [Fact]
    public async Task SendBookingConfirmationAsync_ValidMessage_ReturnsCompletedTask()
    {
        BookingConfirmedMessage message = new(
            BookingId: Guid.NewGuid(),
            GuestEmail: "guest@test.com",
            GuestName: "Juan Pérez",
            HotelName: "Hotel Test",
            RoomType: "Double",
            CheckIn: new DateOnly(2025, 6, 1),
            CheckOut: new DateOnly(2025, 6, 5),
            TotalPrice: 400m);

        await _sut.SendBookingConfirmationAsync(message);
    }
}
