using Microsoft.EntityFrameworkCore;
using UltraHotel.Domain.Entities.Hotels;
using UltraHotel.Infrastructure.Persistence;
using UltraHotel.Infrastructure.Persistence.Rooms;

namespace UltraHotel.Tests.Unit.Infrastructure.Persistence.Rooms;

public class RoomRepositoryTests
{
    private static UltraHotelDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<UltraHotelDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static readonly Guid HotelId = Guid.NewGuid();

    private static Room SampleRoom() =>
        new() { HotelId = HotelId, RoomType = RoomType.Double, BasePrice = 100m, TaxRate = 0.19m, Capacity = 2 };

    [Fact]
    public async Task GetByIdAsync_ExistingRoom_ReturnsIt()
    {
        await using UltraHotelDbContext ctx = CreateContext();
        Room room = SampleRoom();
        ctx.Rooms.Add(room);
        await ctx.SaveChangesAsync();

        Room? result = await new RoomRepository(ctx).GetByIdAsync(room.Id);

        Assert.NotNull(result);
        Assert.Equal(room.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistent_ReturnsNull()
    {
        await using UltraHotelDbContext ctx = CreateContext();

        Room? result = await new RoomRepository(ctx).GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByHotelIdAsync_ReturnsRoomsForHotel()
    {
        await using UltraHotelDbContext ctx = CreateContext();
        ctx.Rooms.AddRange(SampleRoom(), SampleRoom());
        ctx.Rooms.Add(new Room { HotelId = Guid.NewGuid(), RoomType = RoomType.Suite, BasePrice = 200m, TaxRate = 0.19m, Capacity = 1 });
        await ctx.SaveChangesAsync();

        IReadOnlyList<Room> result = await new RoomRepository(ctx).GetByHotelIdAsync(HotelId);

        Assert.Equal(2, result.Count);
        Assert.All(result, r => Assert.Equal(HotelId, r.HotelId));
    }

    [Fact]
    public async Task ExistsByIdAsync_ExistingRoom_ReturnsTrue()
    {
        await using UltraHotelDbContext ctx = CreateContext();
        Room room = SampleRoom();
        ctx.Rooms.Add(room);
        await ctx.SaveChangesAsync();

        bool exists = await new RoomRepository(ctx).ExistsByIdAsync(room.Id);

        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsByIdAsync_NonExistent_ReturnsFalse()
    {
        await using UltraHotelDbContext ctx = CreateContext();

        bool exists = await new RoomRepository(ctx).ExistsByIdAsync(Guid.NewGuid());

        Assert.False(exists);
    }

    [Fact]
    public async Task AddAsync_ValidRoom_PersistsToDatabase()
    {
        await using UltraHotelDbContext ctx = CreateContext();
        Room room = SampleRoom();

        await new RoomRepository(ctx).AddAsync(room);

        Assert.NotNull(await ctx.Rooms.FindAsync(room.Id));
    }

    [Fact]
    public async Task UpdateAsync_ExistingRoom_SavesChanges()
    {
        await using UltraHotelDbContext ctx = CreateContext();
        Room room = SampleRoom();
        ctx.Rooms.Add(room);
        await ctx.SaveChangesAsync();

        room.BasePrice = 250m;
        await new RoomRepository(ctx).UpdateAsync(room);

        Room? updated = await ctx.Rooms.FindAsync(room.Id);
        Assert.Equal(250m, updated!.BasePrice);
    }
}
