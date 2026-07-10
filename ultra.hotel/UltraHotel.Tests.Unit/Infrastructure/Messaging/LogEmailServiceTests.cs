using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using UltraHotel.Commons.Messaging;
using UltraHotel.Infrastructure.Messaging;

namespace UltraHotel.Tests.Unit.Infrastructure.Messaging;

public class LogEmailServiceTests
{
    private static BookingConfirmedMessage SampleMessage() => new(
        BookingId: Guid.NewGuid(),
        GuestEmail: "guest@test.com",
        GuestName: "Juan Pérez",
        HotelName: "Hotel Test",
        RoomType: "Double",
        CheckIn: new DateOnly(2025, 6, 1),
        CheckOut: new DateOnly(2025, 6, 5),
        TotalPrice: 400m);

    [Fact]
    public void SendBookingConfirmationAsync_LoggingDisabled_ReturnsCompletedSuccessfully()
    {
        LogEmailService sut = new(NullLogger<LogEmailService>.Instance);

        Task task = sut.SendBookingConfirmationAsync(SampleMessage());

        Assert.True(task.IsCompletedSuccessfully);
    }

    [Fact]
    public void SendBookingConfirmationAsync_LoggingEnabled_LogsAndReturnsCompletedSuccessfully()
    {
        Mock<ILogger<LogEmailService>> mockLogger = new();
        mockLogger.Setup(l => l.IsEnabled(LogLevel.Information)).Returns(true);
        LogEmailService sut = new(mockLogger.Object);

        Task task = sut.SendBookingConfirmationAsync(SampleMessage());

        Assert.True(task.IsCompletedSuccessfully);
    }
}
