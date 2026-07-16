using CelebrationPassports.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CelebrationPassports.Persistence.Configurations;

public class ChapterConfiguration : IEntityTypeConfiguration<Chapter>
{
    public void Configure(EntityTypeBuilder<Chapter> builder)
    {
        builder.ToTable("Chapters");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(x => x.Status)
            .HasDefaultValue(Enums.ChapterStatus.Confirmed);

        builder.Property(x => x.Source)
            .HasDefaultValue(Enums.ChapterSource.Manual);

        builder.Property(x => x.SongTitle)
            .HasMaxLength(200);

        builder.Property(x => x.SongArtist)
            .HasMaxLength(200);

        builder.Property(x => x.SongLinkUrl)
            .HasMaxLength(500);

        builder.HasIndex(x => new { x.StoryId, x.EventDate });

        builder.HasOne(x => x.Passport)
            .WithMany()
            .HasForeignKey(x => x.PassportId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Story)
            .WithMany(x => x.Chapters)
            .HasForeignKey(x => x.StoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Category)
            .WithMany(x => x.Chapters)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Place)
            .WithMany(x => x.Chapters)
            .HasForeignKey(x => x.PlaceId)
            .OnDelete(DeleteBehavior.Restrict);

        // Cover-media FKs use SetNull — losing the cover photo shouldn't block deleting it.
        builder.HasOne(x => x.CoverMedia)
            .WithMany()
            .HasForeignKey(x => x.CoverMediaId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.DeletedByUser)
            .WithMany(x => x.DeletedChapters)
            .HasForeignKey(x => x.DeletedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
