using UltraHotel.Commons.Messaging;

namespace UltraHotel.Application.Features.Notifications.Contracts;

/// <summary>
/// Abstracción del servicio de envío de correos electrónicos.
/// La implementación de producción llamaría a SendGrid, SES u otro proveedor;
/// la implementación actual (<c>LogEmailService</c>) registra el mensaje en logs.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Envía el correo de confirmación de reserva al huésped principal.
    /// Llamado por el consumer de RabbitMQ al recibir el evento <c>booking.confirmed</c>.
    /// </summary>
    /// <param name="message">Datos de la reserva confirmada.</param>
    /// <param name="ct">Token de cancelación.</param>
    Task SendBookingConfirmationAsync(BookingConfirmedMessage message, CancellationToken ct = default);
}
