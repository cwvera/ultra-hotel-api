using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UltraHotel.Domain.Entities.Bookings;

namespace UltraHotel.Infrastructure.Persistence.Configurations;

public class GuestConfiguration : IEntityTypeConfiguration<Guest>
{
    public void Configure(EntityTypeBuilder<Guest> builder)
    {
        builder.ToTable("Guests");
        builder.HasKey(g => g.Id);

        builder.Property(g => g.BookingId).IsRequired();
        builder.Property(g => g.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(g => g.LastName).HasMaxLength(100).IsRequired();
        builder.Property(g => g.BirthDate).HasColumnType("date").IsRequired();

        builder.Property(g => g.Gender)
            .HasConversion(
                v => v.ToString().ToUpper(),
                s => Enum.Parse<Gender>(s, ignoreCase: true))
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(g => g.DocumentType)
            .HasConversion(
                v => v.ToString().ToUpper(),
                s => Enum.Parse<DocumentType>(s, ignoreCase: true))
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(g => g.DocumentNumber).HasMaxLength(50).IsRequired();
        builder.Property(g => g.Email).HasMaxLength(200).IsRequired();
        builder.Property(g => g.Phone).HasMaxLength(20).IsRequired();
    }
}
