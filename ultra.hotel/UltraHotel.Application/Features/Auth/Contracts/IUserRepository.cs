using UltraHotel.Domain.Entities.Identity;

namespace UltraHotel.Application.Features.Auth.Contracts;

/// <summary>
/// Contrato de persistencia para la entidad <see cref="User"/>.
/// Application define el contrato; Infrastructure lo implementa (inversión de dependencias).
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Busca un usuario por su dirección de correo electrónico.
    /// El email se normaliza a minúsculas antes de la búsqueda.
    /// </summary>
    /// <param name="email">Email en minúsculas del usuario a buscar.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>El usuario si existe; <c>null</c> en caso contrario.</returns>
    Task<User?> FindByEmailAsync(string email, CancellationToken ct = default);

    /// <summary>
    /// Verifica si ya existe un usuario registrado con el email indicado.
    /// </summary>
    /// <param name="email">Email a verificar.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns><c>true</c> si el email ya está registrado.</returns>
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);

    /// <summary>
    /// Persiste un nuevo usuario. El <c>PasswordHash</c> debe estar hasheado antes de llamar este método.
    /// </summary>
    /// <param name="user">Usuario a registrar.</param>
    /// <param name="ct">Token de cancelación.</param>
    Task AddAsync(User user, CancellationToken ct = default);
}
