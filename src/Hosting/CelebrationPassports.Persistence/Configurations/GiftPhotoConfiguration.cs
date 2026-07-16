using CelebrationPassports.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CelebrationPassports.Persistence.Configurations;

public class GiftPhotoConfiguration : IEntityTypeConfiguration<GiftPhoto>
{
    public void Configure(EntityTypeBuilder<GiftPhoto> builder)
    {
        builder.ToTable("GiftPhotos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Url).IsRequired().HasMaxLength(500);
        builder.Property(x => x.UserInsight).HasMaxLength(1000);
        builder.Property(x => x.AiGeneratedInsight).HasMaxLength(1000);

        builder.Property(x => x.CreatedOn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(x => x.GiftDraft)
            .WithMany(d => d.Photos)
            .HasForeignKey(x => x.GiftDraftId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.GiftDraftId, x.DisplayOrder });
    }
}
