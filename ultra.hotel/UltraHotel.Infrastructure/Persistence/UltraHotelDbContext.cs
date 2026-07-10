using System.Reflection;
using Microsoft.EntityFrameworkCore;
using UltraHotel.Domain.Entities.Bookings;
using UltraHotel.Domain.Entities.Hotels;
using UltraHotel.Domain.Entities.Identity;

namespace UltraHotel.Infrastructure.Persistence;

public class UltraHotelDbContext(DbContextOptions<UltraHotelDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Hotel> Hotels => Set<Hotel>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Guest> Guests => Set<Guest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
