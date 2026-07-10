using UltraHotel.Domain.Entities.Identity;

namespace UltraHotel.Application.Features.Auth.Contracts;

/// <summary>
/// Servicio de generación de tokens de acceso (JWT).
/// Application define el contrato; Infrastructure provee la implementación con la clave secreta.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Genera un token JWT firmado para el usuario indicado.
    /// Los claims incluyen <c>sub</c> (userId), <c>email</c> y <c>role</c>.
    /// </summary>
    /// <param name="user">Usuario autenticado para el que se emite el token.</param>
    /// <returns>Token JWT en formato compacto (header.payload.signature).</returns>
    string GenerateToken(User user);
}
