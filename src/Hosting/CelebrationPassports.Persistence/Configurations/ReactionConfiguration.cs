using CelebrationPassports.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CelebrationPassports.Persistence.Configurations;

public class ReactionConfiguration : IEntityTypeConfiguration<Reaction>
{
    public void Configure(EntityTypeBuilder<Reaction> builder)
    {
        builder.ToTable("Reactions", t => t.HasCheckConstraint(
            "CK_Reactions_ExactlyOneTarget",
            "(CASE WHEN \"ChapterId\" IS NOT NULL THEN 1 ELSE 0 END) + " +
            "(CASE WHEN \"MediaId\" IS NOT NULL THEN 1 ELSE 0 END) + " +
            "(CASE WHEN \"CommentId\" IS NOT NULL THEN 1 ELSE 0 END) = 1"));

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Type)
            .IsRequired();

        builder.HasOne(x => x.Chapter)
            .WithMany(x => x.Reactions)
            .HasForeignKey(x => x.ChapterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Media)
            .WithMany(x => x.Reactions)
            .HasForeignKey(x => x.MediaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Comment)
            .WithMany(x => x.Reactions)
            .HasForeignKey(x => x.CommentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Reactions)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
