using Microsoft.EntityFrameworkCore;
using UltraHotel.Domain.Entities.Hotels;
using UltraHotel.Infrastructure.Persistence;
using UltraHotel.Infrastructure.Persistence.Hotels;

namespace UltraHotel.Tests.Unit.Infrastructure.Persistence.Hotels;

public class HotelRepositoryTests
{
    private static UltraHotelDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<UltraHotelDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static Hotel SampleHotel() =>
        new() { Name = "Hotel Test", City = "Bogotá", Address = "Calle 1", AgentEmail = "a@b.com" };

    [Fact]
    public async Task GetByIdAsync_ExistingHotel_ReturnsIt()
    {
        await using UltraHotelDbContext ctx = CreateContext();
        Hotel hotel = SampleHotel();
        ctx.Hotels.Add(hotel);
        await ctx.SaveChangesAsync();

        Hotel? result = await new HotelRepository(ctx).GetByIdAsync(hotel.Id);

        Assert.NotNull(result);
        Assert.Equal(hotel.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistent_ReturnsNull()
    {
        await using UltraHotelDbContext ctx = CreateContext();

        Hotel? result = await new HotelRepository(ctx).GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task ExistsByIdAsync_ExistingHotel_ReturnsTrue()
    {
        await using UltraHotelDbContext ctx = CreateContext();
        Hotel hotel = SampleHotel();
        ctx.Hotels.Add(hotel);
        await ctx.SaveChangesAsync();

        bool exists = await new HotelRepository(ctx).ExistsByIdAsync(hotel.Id);

        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsByIdAsync_NonExistent_ReturnsFalse()
    {
        await using UltraHotelDbContext ctx = CreateContext();

        bool exists = await new HotelRepository(ctx).ExistsByIdAsync(Guid.NewGuid());

        Assert.False(exists);
    }

    [Fact]
    public async Task AddAsync_ValidHotel_PersistsToDatabase()
    {
        await using UltraHotelDbContext ctx = CreateContext();
        Hotel hotel = SampleHotel();

        await new HotelRepository(ctx).AddAsync(hotel);

        Assert.NotNull(await ctx.Hotels.FindAsync(hotel.Id));
    }

    [Fact]
    public async Task UpdateAsync_ExistingHotel_SavesChanges()
    {
        await using UltraHotelDbContext ctx = CreateContext();
        Hotel hotel = SampleHotel();
        ctx.Hotels.Add(hotel);
        await ctx.SaveChangesAsync();

        hotel.Name = "Updated Name";
        await new HotelRepository(ctx).UpdateAsync(hotel);

        Hotel? updated = await ctx.Hotels.FindAsync(hotel.Id);
        Assert.Equal("Updated Name", updated!.Name);
    }
}
