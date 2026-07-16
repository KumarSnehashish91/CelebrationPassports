using CelebrationPassports.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CelebrationPassports.Persistence.Configurations;

public class GuestbookSubmissionConfiguration : IEntityTypeConfiguration<GuestbookSubmission>
{
    public void Configure(EntityTypeBuilder<GuestbookSubmission> builder)
    {
        builder.ToTable("GuestbookSubmissions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.GuestName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Message)
            .HasMaxLength(500);

        builder.Property(x => x.PhotoUrl)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(x => x.Chapter)
            .WithMany()
            .HasForeignKey(x => x.ChapterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ReviewedByUser)
            .WithMany()
            .HasForeignKey(x => x.ReviewedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ApprovedMedia)
            .WithMany()
            .HasForeignKey(x => x.ApprovedMediaId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => new { x.ChapterId, x.Status });
    }
}
