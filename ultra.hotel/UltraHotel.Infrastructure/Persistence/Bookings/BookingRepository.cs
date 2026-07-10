using Microsoft.EntityFrameworkCore;
using UltraHotel.Application.Features.Bookings.Contracts;
using UltraHotel.Domain.Entities.Bookings;

namespace UltraHotel.Infrastructure.Persistence.Bookings;

public class BookingRepository(UltraHotelDbContext db) : IBookingRepository
{
    /// <inheritdoc />
    public Task<Booking?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Bookings.Include(b => b.Guests).AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id, ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<Booking>> GetByHotelIdAsync(Guid hotelId, CancellationToken ct = default)
    {
        List<Booking> bookings = await db.Bookings.Include(b => b.Guests)
            .Where(b => b.HotelId == hotelId)
            .OrderByDescending(b => b.CreatedAt)
            .AsNoTracking()
            .ToListAsync(ct);
        return bookings.AsReadOnly();
    }

    /// <inheritdoc />
    public async Task AddAsync(Booking booking, CancellationToken ct = default)
    {
        db.Bookings.Add(booking);
        await db.SaveChangesAsync(ct);
    }

    /// <inheritdoc />
    public Task<bool> IsRoomAvailableAsync(Guid roomId, DateOnly checkIn, DateOnly checkOut, CancellationToken ct = default) =>
        db.Bookings.AllAsync(b =>
            b.RoomId != roomId ||
            b.Status != BookingStatus.Confirmed ||
            b.CheckOutDate <= checkIn ||
            b.CheckInDate >= checkOut, ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<Guid>> GetUnavailableRoomIdsAsync(
        IEnumerable<Guid> candidateRoomIds,
        DateOnly checkIn,
        DateOnly checkOut,
        CancellationToken ct = default)
    {
        List<Guid> ids = candidateRoomIds.ToList();
        return await db.Bookings
            .Where(b => ids.Contains(b.RoomId) &&
                        b.Status == BookingStatus.Confirmed &&
                        b.CheckInDate < checkOut &&
                        b.CheckOutDate > checkIn)
            .Select(b => b.RoomId)
            .Distinct()
            .ToListAsync(ct);
    }
}
