using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using UltraHotel.Application.Features.Notifications.Contracts;
using UltraHotel.Commons.Messaging;

namespace UltraHotel.Infrastructure.Messaging;

public sealed class BookingConfirmedConsumer(
    IOptions<RabbitMqSettings> options,
    IServiceScopeFactory scopeFactory,
    ILogger<BookingConfirmedConsumer> logger) : BackgroundService
{
    private const string Queue = "booking.confirmed";
    private IConnection? _connection;
    private IChannel? _channel;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            ConnectionFactory factory = new()
            {
                HostName    = options.Value.Host,
                Port        = options.Value.Port,
                UserName    = options.Value.Username,
                Password    = options.Value.Password,
                VirtualHost = options.Value.VirtualHost
            };

            _connection = await factory.CreateConnectionAsync(stoppingToken);
            _channel    = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await _channel.QueueDeclareAsync(Queue, durable: true, exclusive: false,
                autoDelete: false, cancellationToken: stoppingToken);

            AsyncEventingBasicConsumer consumer = new(_channel);
            consumer.ReceivedAsync += OnMessageReceivedAsync;

            await _channel.BasicConsumeAsync(Queue, autoAck: false, consumer: consumer,
                cancellationToken: stoppingToken);

            logger.LogInformation("BookingConfirmedConsumer listening on queue '{Queue}'", Queue);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException) { /* shutdown */ }
        catch (Exception ex)
        {
            logger.LogError(ex, "BookingConfirmedConsumer fatal error");
        }
    }

    private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs ea)
    {
        try
        {
            string                  json    = Encoding.UTF8.GetString(ea.Body.Span);
            BookingConfirmedMessage? message = JsonSerializer.Deserialize<BookingConfirmedMessage>(json);

            if (message is null)
            {
                await _channel!.BasicNackAsync(ea.DeliveryTag, false, false);
                return;
            }

            using IServiceScope scope   = scopeFactory.CreateScope();
            IEmailService emailService  = scope.ServiceProvider.GetRequiredService<IEmailService>();

            await emailService.SendBookingConfirmationAsync(message);

            await _channel!.BasicAckAsync(ea.DeliveryTag, false);
            logger.LogInformation("Booking confirmation email sent to {Email}", message.GuestEmail);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process booking confirmed message");
            await _channel!.BasicNackAsync(ea.DeliveryTag, false, requeue: true);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
        if (_channel is not null) await _channel.CloseAsync();
        if (_connection is not null) await _connection.CloseAsync();
    }
}
