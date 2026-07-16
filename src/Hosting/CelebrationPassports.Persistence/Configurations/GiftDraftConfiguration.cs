using CelebrationPassports.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CelebrationPassports.Persistence.Configurations;

public class GiftDraftConfiguration : IEntityTypeConfiguration<GiftDraft>
{
    public void Configure(EntityTypeBuilder<GiftDraft> builder)
    {
        builder.ToTable("GiftDrafts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.RecipientName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.RecipientEmail).HasMaxLength(256);
        builder.Property(x => x.PassportTitle).HasMaxLength(200);
        builder.Property(x => x.PersonalMessage).HasMaxLength(1000);
        builder.Property(x => x.MessageMediaUrl).HasMaxLength(500);

        builder.Property(x => x.CreatedOn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(x => x.SenderUser)
            .WithMany(u => u.SentGiftDrafts)
            .HasForeignKey(x => x.SenderUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // One-to-one, but the FK lives on PassportGift's own PassportId uniqueness
        // already enforces "one gift per passport" — here we just need "one draft
        // finalizes into at most one gift".
        builder.HasOne(x => x.PassportGift)
            .WithOne()
            .HasForeignKey<GiftDraft>(x => x.PassportGiftId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.PassportGiftId).IsUnique().HasFilter("\"PassportGiftId\" IS NOT NULL");
    }
}
