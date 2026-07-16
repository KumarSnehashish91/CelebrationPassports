using CelebrationPassports.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CelebrationPassports.Persistence.Configurations;

public class ChapterInvitationConfiguration : IEntityTypeConfiguration<ChapterInvitation>
{
    public void Configure(EntityTypeBuilder<ChapterInvitation> builder)
    {
        builder.ToTable("ChapterInvitations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.HasOne(x => x.Chapter)
            .WithMany()
            .HasForeignKey(x => x.ChapterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.InvitedByUser)
            .WithMany()
            .HasForeignKey(x => x.InvitedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.ChapterId, x.Status });
    }
}
