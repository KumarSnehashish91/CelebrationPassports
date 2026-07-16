using CelebrationPassports.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CelebrationPassports.Persistence.Configurations;

public class ChapterContributorConfiguration : IEntityTypeConfiguration<ChapterContributor>
{
    public void Configure(EntityTypeBuilder<ChapterContributor> builder)
    {
        builder.ToTable("ChapterContributors");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(x => x.Chapter)
            .WithMany()
            .HasForeignKey(x => x.ChapterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.InvitedByUser)
            .WithMany()
            .HasForeignKey(x => x.InvitedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.ChapterId, x.UserId }).IsUnique();
    }
}
