using Microsoft.Extensions.Logging;
using UltraHotel.Application.Features.Notifications.Contracts;
using UltraHotel.Commons.Messaging;

namespace UltraHotel.Infrastructure.Messaging;

public partial class LogEmailService(ILogger<LogEmailService> logger) : IEmailService
{
    /// <inheritdoc/>
    public Task SendBookingConfirmationAsync(BookingConfirmedMessage message, CancellationToken ct = default)
    {
        LogEmailSent(logger, message.GuestEmail, message.GuestName, message.HotelName,
            message.RoomType, message.CheckIn, message.CheckOut, message.TotalPrice);

        return Task.CompletedTask;
    }

    [LoggerMessage(Level = LogLevel.Information,
        Message = "[EMAIL] To={GuestEmail} | Guest={GuestName} | Hotel={HotelName} | Room={RoomType} | {CheckIn} → {CheckOut} | Total={TotalPrice}")]
    private static partial void LogEmailSent(ILogger logger, string guestEmail, string guestName,
        string hotelName, string roomType, DateOnly checkIn, DateOnly checkOut, decimal totalPrice);
}
