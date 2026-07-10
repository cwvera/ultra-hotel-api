using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using UltraHotel.Commons.Contracts;

namespace UltraHotel.Infrastructure.Messaging;

public sealed class RabbitMqPublisher : IMessagePublisher, IAsyncDisposable
{
    private readonly ILogger<RabbitMqPublisher> _logger;
    private readonly RabbitMqSettings _settings;
    private readonly ResiliencePipeline _pipeline;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqPublisher(IOptions<RabbitMqSettings> options, ILogger<RabbitMqPublisher> logger)
    {
        _settings = options.Value;
        _logger = logger;
        _pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(2),
                BackoffType = DelayBackoffType.Exponential,
                ShouldHandle = args => ValueTask.FromResult(args.Outcome.Exception is not null),
                OnRetry = args =>
                {
                    _logger.LogWarning("RabbitMQ publish retry #{Attempt} — {Exception}",
                        args.AttemptNumber + 1, args.Outcome.Exception?.Message);
                    _channel = null;
                    return default;
                }
            })
            .Build();
    }

    private async Task EnsureChannelAsync(CancellationToken ct)
    {
        if (_channel is { IsOpen: true })
        {
            return;
        }

        await _initLock.WaitAsync(ct);
        try
        {
            if (_channel is { IsOpen: true })
            {
                return;
            }

            if (_channel is not null) { try { await _channel.CloseAsync(); } catch { /* stale */ } }
            if (_connection is not null) { try { await _connection.CloseAsync(); } catch { /* stale */ } }
            _channel = null;
            _connection = null;

            ConnectionFactory factory = new()
            {
                HostName = _settings.Host,
                Port = _settings.Port,
                UserName = _settings.Username,
                Password = _settings.Password,
                VirtualHost = _settings.VirtualHost
            };

            _connection = await factory.CreateConnectionAsync(ct);
            _channel = await _connection.CreateChannelAsync(cancellationToken: ct);

            _logger.LogInformation("RabbitMQ connected to {Host}:{Port}", _settings.Host, _settings.Port);
        }
        finally
        {
            _initLock.Release();
        }
    }

    /// <inheritdoc/>
    public async Task PublishAsync<T>(string queue, T message, CancellationToken ct = default) where T : class
    {
        await _pipeline.ExecuteAsync(async innerCt =>
        {
            await EnsureChannelAsync(innerCt);

            await _channel!.QueueDeclareAsync(queue, durable: true, exclusive: false,
                autoDelete: false, cancellationToken: innerCt);

            byte[] body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            BasicProperties props = new() { Persistent = true };

            await _channel.BasicPublishAsync(exchange: string.Empty, routingKey: queue,
                mandatory: false, basicProperties: props, body: body, cancellationToken: innerCt);

            _logger.LogInformation("Published message to queue '{Queue}'", queue);
        }, ct);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
        {
            await _channel.CloseAsync();
        }

        if (_connection is not null)
        {
            await _connection.CloseAsync();
        }
    }
}
