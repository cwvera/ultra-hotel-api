using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UltraHotel.Domain.Entities.Hotels;

namespace UltraHotel.Infrastructure.Persistence.Configurations;

public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
{
    public void Configure(EntityTypeBuilder<Hotel> builder)
    {
        builder.ToTable("Hotels");
        builder.HasKey(h => h.Id);

        builder.Property(h => h.Name).HasMaxLength(200).IsRequired();
        builder.Property(h => h.City).HasMaxLength(100).IsRequired();
        builder.Property(h => h.Address).HasMaxLength(300).IsRequired();
        builder.Property(h => h.Description).HasMaxLength(1000);
        builder.Property(h => h.AgentEmail).HasMaxLength(200).IsRequired();
        builder.Property(h => h.IsEnabled).IsRequired();
        builder.Property(h => h.CreatedAt).IsRequired();
        builder.Property(h => h.UpdatedAt).IsRequired();
    }
}
