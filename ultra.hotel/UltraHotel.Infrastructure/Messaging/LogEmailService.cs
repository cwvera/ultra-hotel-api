using Microsoft.Extensions.Logging;
using UltraHotel.Application.Features.Notifications.Contracts;
using UltraHotel.Commons.Messaging;

namespace UltraHotel.Infrastructure.Messaging;

public class LogEmailService(ILogger<LogEmailService> logger) : IEmailService
{
    /// <inheritdoc/>
    public Task SendBookingConfirmationAsync(BookingConfirmedMessage message, CancellationToken ct = default)
    {
        logger.LogInformation(
            "[EMAIL] To={Email} | Guest={Guest} | Hotel={Hotel} | Room={Room} | {CheckIn} → {CheckOut} | Total={Total:C}",
            message.GuestEmail, message.GuestName, message.HotelName,
            message.RoomType, message.CheckIn, message.CheckOut, message.TotalPrice);

        return Task.CompletedTask;
    }
}
