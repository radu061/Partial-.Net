using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Infrastructure.Persistence.Configurations;

public sealed class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).ValueGeneratedNever();

        builder.Property(b => b.BookedBy).IsRequired().HasMaxLength(200);

        builder.Property(b => b.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Ignore(b => b.IsActive);

        builder.ComplexProperty(b => b.Range, range =>
        {
            range.Property(r => r.StartUtc)
                .HasColumnName("StartUtc")
                .HasConversion<UtcDateTimeConverter>();
            range.Property(r => r.EndUtc)
                .HasColumnName("EndUtc")
                .HasConversion<UtcDateTimeConverter>();
            range.Ignore(r => r.Duration);
        });

        builder.HasOne<Room>()
            .WithMany()
            .HasForeignKey(b => b.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(b => b.RoomId);
    }
}
