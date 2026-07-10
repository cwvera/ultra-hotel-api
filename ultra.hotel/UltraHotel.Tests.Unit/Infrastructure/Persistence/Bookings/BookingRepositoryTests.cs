using Microsoft.EntityFrameworkCore;
using UltraHotel.Domain.Entities.Bookings;
using UltraHotel.Infrastructure.Persistence;
using UltraHotel.Infrastructure.Persistence.Bookings;

namespace UltraHotel.Tests.Unit.Infrastructure.Persistence.Bookings;

public class BookingRepositoryTests
{
    private static UltraHotelDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<UltraHotelDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static readonly Guid HotelId = Guid.NewGuid();
    private static readonly Guid RoomId = Guid.NewGuid();

    private static Booking ConfirmedBooking(DateOnly checkIn, DateOnly checkOut) => new()
    {
        HotelId = HotelId,
        RoomId = RoomId,
        Status = BookingStatus.Confirmed,
        CheckInDate = checkIn,
        CheckOutDate = checkOut,
        TotalPrice = 300m,
        EmergencyContactName = "E",
        EmergencyContactPhone = "555"
    };

    [Fact]
    public async Task GetByIdAsync_ExistingBooking_ReturnsWithGuests()
    {
        await using UltraHotelDbContext ctx = CreateContext();
        Booking booking = ConfirmedBooking(new DateOnly(2025, 6, 1), new DateOnly(2025, 6, 4));
        ctx.Bookings.Add(booking);
        await ctx.SaveChangesAsync();

        Booking? result = await new BookingRepository(ctx).GetByIdAsync(booking.Id);

        Assert.NotNull(result);
        Assert.Equal(booking.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistent_ReturnsNull()
    {
        await using UltraHotelDbContext ctx = CreateContext();

        Booking? result = await new BookingRepository(ctx).GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByHotelIdAsync_ReturnsBookingsForHotel()
    {
        await using UltraHotelDbContext ctx = CreateContext();
        ctx.Bookings.Add(ConfirmedBooking(new DateOnly(2025, 6, 1), new DateOnly(2025, 6, 3)));
        ctx.Bookings.Add(ConfirmedBooking(new DateOnly(2025, 7, 1), new DateOnly(2025, 7, 5)));
        ctx.Bookings.Add(new Booking { HotelId = Guid.NewGuid(), RoomId = Guid.NewGuid(), Status = BookingStatus.Confirmed, CheckInDate = new DateOnly(2025, 8, 1), CheckOutDate = new DateOnly(2025, 8, 3), TotalPrice = 100m });
        await ctx.SaveChangesAsync();

        IReadOnlyList<Booking> result = await new BookingRepository(ctx).GetByHotelIdAsync(HotelId);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task AddAsync_ValidBooking_PersistsToDatabase()
    {
        await using UltraHotelDbContext ctx = CreateContext();
        Booking booking = ConfirmedBooking(new DateOnly(2025, 6, 1), new DateOnly(2025, 6, 4));

        await new BookingRepository(ctx).AddAsync(booking);

        Assert.NotNull(await ctx.Bookings.FindAsync(booking.Id));
    }

    [Fact]
    public async Task IsRoomAvailableAsync_NoConflict_ReturnsTrue()
    {
        await using UltraHotelDbContext ctx = CreateContext();
        ctx.Bookings.Add(ConfirmedBooking(new DateOnly(2025, 6, 1), new DateOnly(2025, 6, 5)));
        await ctx.SaveChangesAsync();

        bool available = await new BookingRepository(ctx).IsRoomAvailableAsync(
            RoomId, new DateOnly(2025, 6, 6), new DateOnly(2025, 6, 10));

        Assert.True(available);
    }

    [Fact]
    public async Task IsRoomAvailableAsync_OverlappingBooking_ReturnsFalse()
    {
        await using UltraHotelDbContext ctx = CreateContext();
        ctx.Bookings.Add(ConfirmedBooking(new DateOnly(2025, 6, 1), new DateOnly(2025, 6, 10)));
        await ctx.SaveChangesAsync();

        bool available = await new BookingRepository(ctx).IsRoomAvailableAsync(
            RoomId, new DateOnly(2025, 6, 5), new DateOnly(2025, 6, 8));

        Assert.False(available);
    }

    [Fact]
    public async Task GetUnavailableRoomIdsAsync_OverlappingBookings_ReturnsConflictingIds()
    {
        await using UltraHotelDbContext ctx = CreateContext();
        Guid otherRoomId = Guid.NewGuid();
        ctx.Bookings.Add(ConfirmedBooking(new DateOnly(2025, 6, 1), new DateOnly(2025, 6, 10)));
        await ctx.SaveChangesAsync();

        IReadOnlyList<Guid> result = await new BookingRepository(ctx).GetUnavailableRoomIdsAsync(
            [RoomId, otherRoomId], new DateOnly(2025, 6, 5), new DateOnly(2025, 6, 8));

        Assert.Single(result);
        Assert.Equal(RoomId, result[0]);
    }

    [Fact]
    public async Task GetUnavailableRoomIdsAsync_NoOverlap_ReturnsEmpty()
    {
        await using UltraHotelDbContext ctx = CreateContext();
        ctx.Bookings.Add(ConfirmedBooking(new DateOnly(2025, 6, 1), new DateOnly(2025, 6, 5)));
        await ctx.SaveChangesAsync();

        IReadOnlyList<Guid> result = await new BookingRepository(ctx).GetUnavailableRoomIdsAsync(
            [RoomId], new DateOnly(2025, 6, 6), new DateOnly(2025, 6, 10));

        Assert.Empty(result);
    }
}
