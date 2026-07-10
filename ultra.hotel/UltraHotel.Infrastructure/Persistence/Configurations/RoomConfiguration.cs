using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UltraHotel.Domain.Entities.Hotels;

namespace UltraHotel.Infrastructure.Persistence.Configurations;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("Rooms");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.HotelId).IsRequired();

        builder.Property(r => r.RoomType)
            .HasConversion(
                rt => rt.ToString().ToUpper(),
                s => Enum.Parse<RoomType>(s, ignoreCase: true))
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(r => r.BasePrice).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(r => r.TaxRate).HasColumnType("decimal(5,4)").IsRequired();
        builder.Property(r => r.Capacity).IsRequired();
        builder.Property(r => r.LocationInHotel).HasMaxLength(200);
        builder.Property(r => r.IsEnabled).IsRequired();
        builder.Property(r => r.CreatedAt).IsRequired();
        builder.Property(r => r.UpdatedAt).IsRequired();

        // TotalPrice is a computed C# property; EF must ignore it
        builder.Ignore(r => r.TotalPrice);
    }
}
