using CelebrationPassports.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CelebrationPassports.Persistence.Configurations;

public class GeneratedStoryConfiguration : IEntityTypeConfiguration<GeneratedStory>
{
    public void Configure(EntityTypeBuilder<GeneratedStory> builder)
    {
        builder.ToTable("GeneratedStories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
        builder.Property(x => x.OpeningParagraph).IsRequired();
        builder.Property(x => x.ClosingParagraph).IsRequired();
        builder.Property(x => x.PullQuoteText).HasMaxLength(500);
        builder.Property(x => x.BodyParagraphsJson).IsRequired();

        builder.Property(x => x.GeneratedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(x => x.GiftDraft)
            .WithOne(d => d.Story)
            .HasForeignKey<GeneratedStory>(x => x.GiftDraftId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.GiftDraftId).IsUnique();
    }
}
