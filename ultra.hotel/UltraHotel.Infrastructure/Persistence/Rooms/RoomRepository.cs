using Microsoft.EntityFrameworkCore;
using UltraHotel.Application.Features.Rooms.Contracts;
using UltraHotel.Domain.Entities.Hotels;

namespace UltraHotel.Infrastructure.Persistence.Rooms;

public class RoomRepository(UltraHotelDbContext db) : IRoomRepository
{
    /// <inheritdoc />
    public Task<Room?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Rooms.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id, ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<Room>> GetByHotelIdAsync(Guid hotelId, CancellationToken ct = default)
    {
        List<Room> rooms = await db.Rooms.AsNoTracking()
            .Where(r => r.HotelId == hotelId)
            .ToListAsync(ct);
        return rooms.AsReadOnly();
    }

    /// <inheritdoc />
    public Task<bool> ExistsByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Rooms.AnyAsync(r => r.Id == id, ct);

    /// <inheritdoc />
    public async Task AddAsync(Room room, CancellationToken ct = default)
    {
        db.Rooms.Add(room);
        await db.SaveChangesAsync(ct);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Room room, CancellationToken ct = default)
    {
        db.Rooms.Update(room);
        await db.SaveChangesAsync(ct);
    }
}
