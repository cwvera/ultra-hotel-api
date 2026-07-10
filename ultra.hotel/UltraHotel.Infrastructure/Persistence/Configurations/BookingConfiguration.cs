using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UltraHotel.Domain.Entities.Bookings;

namespace UltraHotel.Infrastructure.Persistence.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.RoomId).IsRequired();
        builder.Property(b => b.HotelId).IsRequired();

        builder.Property(b => b.CheckInDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(b => b.CheckOutDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(b => b.Status)
            .HasConversion(
                s => s.ToString().ToUpper(),
                s => Enum.Parse<BookingStatus>(s, ignoreCase: true))
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(b => b.TotalPrice).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(b => b.EmergencyContactName).HasMaxLength(200).IsRequired();
        builder.Property(b => b.EmergencyContactPhone).HasMaxLength(20).IsRequired();
        builder.Property(b => b.CreatedAt).IsRequired();

        builder.Ignore(b => b.NumberOfNights);

        builder.HasMany(b => b.Guests)
            .WithOne()
            .HasForeignKey(g => g.BookingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
