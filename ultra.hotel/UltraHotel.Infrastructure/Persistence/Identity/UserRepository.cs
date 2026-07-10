using Microsoft.EntityFrameworkCore;
using UltraHotel.Application.Features.Auth.Contracts;
using UltraHotel.Domain.Entities.Identity;

namespace UltraHotel.Infrastructure.Persistence.Identity;

public class UserRepository(UltraHotelDbContext context) : IUserRepository
{
    /// <inheritdoc />
    public Task<User?> FindByEmailAsync(string email, CancellationToken ct = default) =>
        context.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, ct);

    /// <inheritdoc />
    public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default) =>
        context.Users.AnyAsync(u => u.Email == email, ct);

    /// <inheritdoc />
    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        await context.Users.AddAsync(user, ct);
        await context.SaveChangesAsync(ct);
    }
}
