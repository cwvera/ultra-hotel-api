using Microsoft.EntityFrameworkCore;
using UltraHotel.Application.Features.Hotels.Contracts;
using UltraHotel.Domain.Entities.Hotels;

namespace UltraHotel.Infrastructure.Persistence.Hotels;

public class HotelRepository(UltraHotelDbContext db) : IHotelRepository
{
    /// <inheritdoc />
    public Task<Hotel?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Hotels.AsNoTracking().FirstOrDefaultAsync(h => h.Id == id, ct);

    /// <inheritdoc />
    public Task<bool> ExistsByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Hotels.AnyAsync(h => h.Id == id, ct);

    /// <inheritdoc />
    public async Task AddAsync(Hotel hotel, CancellationToken ct = default)
    {
        db.Hotels.Add(hotel);
        await db.SaveChangesAsync(ct);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Hotel hotel, CancellationToken ct = default)
    {
        db.Hotels.Update(hotel);
        await db.SaveChangesAsync(ct);
    }
}
