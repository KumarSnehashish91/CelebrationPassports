using CelebrationPassports.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CelebrationPassports.Persistence.Configurations;

public class SomedayIdeaConfiguration : IEntityTypeConfiguration<SomedayIdea>
{
    public void Configure(EntityTypeBuilder<SomedayIdea> builder)
    {
        builder.ToTable("SomedayIdeas");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Notes)
            .HasMaxLength(2000);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

        builder.HasOne(x => x.Passport)
            .WithMany(x => x.SomedayIdeas)
            .HasForeignKey(x => x.PassportId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany(x => x.CreatedSomedayIdeas)
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Losing the linked Event shouldn't block deleting it — the idea just reverts to
        // looking unconverted.
        builder.HasOne(x => x.ConvertedToEvent)
            .WithMany()
            .HasForeignKey(x => x.ConvertedToEventId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.DeletedByUser)
            .WithMany(x => x.DeletedSomedayIdeas)
            .HasForeignKey(x => x.DeletedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
