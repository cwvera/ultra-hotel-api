using UltraHotel.Domain.Entities.Hotels;

namespace UltraHotel.Application.Features.Hotels.Contracts;

/// <summary>
/// Contrato de persistencia para la entidad <see cref="Hotel"/>.
/// Application define el contrato; Infrastructure lo implementa (inversión de dependencias).
/// </summary>
public interface IHotelRepository
{
    /// <summary>
    /// Obtiene un hotel por su identificador único.
    /// </summary>
    /// <param name="id">Identificador del hotel.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>El hotel si existe; <c>null</c> en caso contrario.</returns>
    Task<Hotel?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Verifica si existe un hotel con el identificador indicado.
    /// </summary>
    /// <param name="id">Identificador a verificar.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns><c>true</c> si el hotel existe.</returns>
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Persiste un nuevo hotel en la base de datos.
    /// </summary>
    /// <param name="hotel">Hotel a crear.</param>
    /// <param name="ct">Token de cancelación.</param>
    Task AddAsync(Hotel hotel, CancellationToken ct = default);

    /// <summary>
    /// Actualiza los datos de un hotel existente.
    /// El objeto <paramref name="hotel"/> debe tener el <c>Id</c> asignado.
    /// </summary>
    /// <param name="hotel">Hotel con los datos actualizados.</param>
    /// <param name="ct">Token de cancelación.</param>
    Task UpdateAsync(Hotel hotel, CancellationToken ct = default);
}
