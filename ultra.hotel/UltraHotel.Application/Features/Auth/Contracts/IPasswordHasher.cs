namespace UltraHotel.Application.Features.Auth.Contracts;

/// <summary>
/// Abstracción para hasheo y verificación de contraseñas.
/// El algoritmo concreto (BCrypt, Argon2, etc.) lo elige Infrastructure.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Genera un hash seguro a partir de una contraseña en texto plano.
    /// </summary>
    /// <param name="password">Contraseña en texto plano.</param>
    /// <returns>Hash de la contraseña listo para persistir.</returns>
    string Hash(string password);

    /// <summary>
    /// Verifica si una contraseña en texto plano coincide con su hash almacenado.
    /// </summary>
    /// <param name="password">Contraseña en texto plano introducida por el usuario.</param>
    /// <param name="hash">Hash almacenado en base de datos.</param>
    /// <returns><c>true</c> si la contraseña es correcta.</returns>
    bool Verify(string password, string hash);
}
