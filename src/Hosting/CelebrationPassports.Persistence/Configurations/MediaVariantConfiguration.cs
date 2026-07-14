using CelebrationPassports.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CelebrationPassports.Persistence.Configurations;

public class MediaVariantConfiguration : IEntityTypeConfiguration<MediaVariant>
{
    public void Configure(EntityTypeBuilder<MediaVariant> builder)
    {
        builder.ToTable("MediaVariants");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.VariantType)
            .IsRequired();

        builder.Property(x => x.Url)
            .IsRequired()
            .HasMaxLength(1000);

        // Pure derivative of Media — always created/removed alongside its parent, unlike
        // every other relationship in this schema (Restrict).
        builder.HasOne(x => x.Media)
            .WithMany(x => x.Variants)
            .HasForeignKey(x => x.MediaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
