using UltraHotel.Application.Features.Auth.Contracts;

namespace UltraHotel.Infrastructure.Security;

public class BcryptPasswordHasher : IPasswordHasher
{
    /// <inheritdoc />
    public string Hash(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

    /// <inheritdoc />
    public bool Verify(string password, string hash) =>
        BCrypt.Net.BCrypt.Verify(password, hash);
}
