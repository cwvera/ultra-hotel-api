using UltraHotel.Application.Features.Search.Dtos;

namespace UltraHotel.Application.Features.Search.Contracts;

/// <summary>
/// Abstracción del motor de búsqueda de habitaciones (Elasticsearch).
/// Application define el contrato; Infrastructure provee el cliente real.
/// Los fallos de indexación son no-fatales: se registran y no propagan al flujo principal.
/// </summary>
public interface IElasticsearchService
{
    /// <summary>
    /// Indexa o re-indexa un documento de habitación en Elasticsearch.
    /// Se invoca al crear, actualizar o cambiar el estado de una habitación o su hotel.
    /// </summary>
    /// <param name="document">Documento con toda la información de la habitación para búsqueda.</param>
    /// <param name="ct">Token de cancelación.</param>
    Task IndexRoomAsync(RoomIndexDocument document, CancellationToken ct = default);

    /// <summary>
    /// Busca habitaciones disponibles por ciudad y capacidad mínima.
    /// Filtra por <c>hotel_enabled</c>, <c>room_enabled</c> e <c>is_available</c>.
    /// La exclusión de fechas no disponibles se realiza en la capa de aplicación.
    /// </summary>
    /// <param name="city">Ciudad en minúsculas (normalizada por el handler antes de llamar).</param>
    /// <param name="minCapacity">Capacidad mínima requerida.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Lista de habitaciones candidatas; vacía si no hay resultados o si ES no está disponible.</returns>
    Task<IReadOnlyList<RoomSearchResult>> SearchRoomsAsync(string city, int minCapacity, CancellationToken ct = default);

    /// <summary>
    /// Elimina el documento de una habitación de todos los índices.
    /// Se invoca únicamente cuando la habitación es eliminada físicamente del sistema.
    /// Para deshabilitar una habitación usar <see cref="IndexRoomAsync"/> con <c>RoomEnabled = false</c>.
    /// </summary>
    /// <param name="roomId">Identificador de la habitación a eliminar del índice.</param>
    /// <param name="ct">Token de cancelación.</param>
    Task DeleteRoomAsync(Guid roomId, CancellationToken ct = default);
}
