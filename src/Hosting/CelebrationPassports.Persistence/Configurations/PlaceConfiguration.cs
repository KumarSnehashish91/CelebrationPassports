using CelebrationPassports.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CelebrationPassports.Persistence.Configurations;

public class PlaceConfiguration : IEntityTypeConfiguration<Place>
{
    public void Configure(EntityTypeBuilder<Place> builder)
    {
        builder.ToTable("Places");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Address)
            .HasMaxLength(300);

        builder.Property(x => x.City)
            .HasMaxLength(200);

        builder.Property(x => x.PostalCode)
            .HasMaxLength(20);

        builder.Property(x => x.Country)
            .HasMaxLength(200);

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.Property(x => x.Latitude)
            .HasColumnType("decimal(9,6)");

        builder.Property(x => x.Longitude)
            .HasColumnType("decimal(9,6)");
    }
}
