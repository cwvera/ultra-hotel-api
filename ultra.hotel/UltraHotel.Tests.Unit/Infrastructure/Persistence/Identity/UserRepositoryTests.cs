using Microsoft.EntityFrameworkCore;
using UltraHotel.Domain.Entities.Identity;
using UltraHotel.Infrastructure.Persistence;
using UltraHotel.Infrastructure.Persistence.Identity;

namespace UltraHotel.Tests.Unit.Infrastructure.Persistence.Identity;

public class UserRepositoryTests
{
    private static UltraHotelDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<UltraHotelDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task FindByEmailAsync_ExistingEmail_ReturnsUser()
    {
        await using UltraHotelDbContext ctx = CreateContext();
        ctx.Users.Add(new User { Email = "test@test.com", PasswordHash = "h", Role = Role.Agent });
        await ctx.SaveChangesAsync();

        User? result = await new UserRepository(ctx).FindByEmailAsync("test@test.com");

        Assert.NotNull(result);
        Assert.Equal("test@test.com", result.Email);
    }

    [Fact]
    public async Task FindByEmailAsync_NonExistentEmail_ReturnsNull()
    {
        await using UltraHotelDbContext ctx = CreateContext();

        User? result = await new UserRepository(ctx).FindByEmailAsync("nobody@test.com");

        Assert.Null(result);
    }

    [Fact]
    public async Task ExistsByEmailAsync_ExistingEmail_ReturnsTrue()
    {
        await using UltraHotelDbContext ctx = CreateContext();
        ctx.Users.Add(new User { Email = "exists@test.com", PasswordHash = "h", Role = Role.Agent });
        await ctx.SaveChangesAsync();

        bool result = await new UserRepository(ctx).ExistsByEmailAsync("exists@test.com");

        Assert.True(result);
    }

    [Fact]
    public async Task ExistsByEmailAsync_NonExistentEmail_ReturnsFalse()
    {
        await using UltraHotelDbContext ctx = CreateContext();

        bool result = await new UserRepository(ctx).ExistsByEmailAsync("nobody@test.com");

        Assert.False(result);
    }

    [Fact]
    public async Task AddAsync_ValidUser_PersistsToDatabase()
    {
        await using UltraHotelDbContext ctx = CreateContext();
        User user = new() { Email = "new@test.com", PasswordHash = "h", Role = Role.Admin };

        await new UserRepository(ctx).AddAsync(user);

        User? saved = await ctx.Users.FindAsync(user.Id);
        Assert.NotNull(saved);
        Assert.Equal("new@test.com", saved.Email);
    }
}
