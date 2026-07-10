using UltraHotel.Domain.Entities.Hotels;

namespace UltraHotel.Application.Features.Rooms.Contracts;

/// <summary>
/// Contrato de persistencia para la entidad <see cref="Room"/>.
/// Application define el contrato; Infrastructure lo implementa (inversión de dependencias).
/// </summary>
public interface IRoomRepository
{
    /// <summary>
    /// Obtiene una habitación por su identificador único.
    /// </summary>
    /// <param name="id">Identificador de la habitación.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>La habitación si existe; <c>null</c> en caso contrario.</returns>
    Task<Room?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Obtiene todas las habitaciones asociadas a un hotel.
    /// </summary>
    /// <param name="hotelId">Identificador del hotel.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Lista (posiblemente vacía) de habitaciones del hotel.</returns>
    Task<IReadOnlyList<Room>> GetByHotelIdAsync(Guid hotelId, CancellationToken ct = default);

    /// <summary>
    /// Verifica si existe una habitación con el identificador indicado.
    /// </summary>
    /// <param name="id">Identificador a verificar.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns><c>true</c> si la habitación existe.</returns>
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Persiste una nueva habitación en la base de datos.
    /// </summary>
    /// <param name="room">Habitación a crear.</param>
    /// <param name="ct">Token de cancelación.</param>
    Task AddAsync(Room room, CancellationToken ct = default);

    /// <summary>
    /// Actualiza los datos de una habitación existente.
    /// El objeto <paramref name="room"/> debe tener el <c>Id</c> asignado.
    /// </summary>
    /// <param name="room">Habitación con los datos actualizados.</param>
    /// <param name="ct">Token de cancelación.</param>
    Task UpdateAsync(Room room, CancellationToken ct = default);
}
