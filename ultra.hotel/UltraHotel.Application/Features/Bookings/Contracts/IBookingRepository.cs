using UltraHotel.Domain.Entities.Bookings;

namespace UltraHotel.Application.Features.Bookings.Contracts;

/// <summary>
/// Contrato de persistencia para la entidad <see cref="Booking"/>.
/// Application define el contrato; Infrastructure lo implementa (inversión de dependencias).
/// </summary>
public interface IBookingRepository
{
    /// <summary>
    /// Obtiene una reserva por su identificador único, incluyendo los huéspedes asociados.
    /// </summary>
    /// <param name="id">Identificador de la reserva.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>La reserva si existe; <c>null</c> en caso contrario.</returns>
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Obtiene todas las reservas asociadas a un hotel (para visualización del agente).
    /// </summary>
    /// <param name="hotelId">Identificador del hotel.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Lista de reservas del hotel, ordenadas por fecha de creación descendente.</returns>
    Task<IReadOnlyList<Booking>> GetByHotelIdAsync(Guid hotelId, CancellationToken ct = default);

    /// <summary>
    /// Persiste una nueva reserva junto con sus huéspedes.
    /// </summary>
    /// <param name="booking">Reserva a crear, con la colección <c>Guests</c> poblada.</param>
    /// <param name="ct">Token de cancelación.</param>
    Task AddAsync(Booking booking, CancellationToken ct = default);

    /// <summary>
    /// Verifica si una habitación está disponible en el rango de fechas indicado.
    /// Una habitación no está disponible si existe otra reserva confirmada que se solape con las fechas.
    /// </summary>
    /// <param name="roomId">Identificador de la habitación.</param>
    /// <param name="checkIn">Fecha de entrada solicitada.</param>
    /// <param name="checkOut">Fecha de salida solicitada.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns><c>true</c> si la habitación está libre en el período.</returns>
    Task<bool> IsRoomAvailableAsync(Guid roomId, DateOnly checkIn, DateOnly checkOut, CancellationToken ct = default);

    /// <summary>
    /// Filtra de una lista de habitaciones candidatas cuáles tienen reservas confirmadas
    /// que se solapan con el rango de fechas indicado (usado por el motor de búsqueda).
    /// </summary>
    /// <param name="candidateRoomIds">IDs de habitaciones a evaluar.</param>
    /// <param name="checkIn">Fecha de entrada solicitada.</param>
    /// <param name="checkOut">Fecha de salida solicitada.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Subconjunto de IDs no disponibles en el período.</returns>
    Task<IReadOnlyList<Guid>> GetUnavailableRoomIdsAsync(
        IEnumerable<Guid> candidateRoomIds, DateOnly checkIn, DateOnly checkOut, CancellationToken ct = default);
}
