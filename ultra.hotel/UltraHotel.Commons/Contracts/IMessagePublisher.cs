namespace UltraHotel.Commons.Contracts;

/// <summary>
/// Abstracción del broker de mensajería para publicación asíncrona de eventos.
/// La implementación actual usa RabbitMQ; puede reemplazarse por Azure Service Bus u otro.
/// </summary>
public interface IMessagePublisher
{
    /// <summary>
    /// Publica un mensaje en la cola indicada de forma asíncrona con confirmación de entrega (durable).
    /// Incluye reintentos automáticos con backoff exponencial ante fallos transitorios.
    /// </summary>
    /// <typeparam name="T">Tipo del mensaje (se serializa como JSON).</typeparam>
    /// <param name="queue">Nombre de la cola destino.</param>
    /// <param name="message">Mensaje a publicar.</param>
    /// <param name="ct">Token de cancelación.</param>
    Task PublishAsync<T>(string queue, T message, CancellationToken ct = default) where T : class;
}
