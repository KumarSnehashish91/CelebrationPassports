using CelebrationPassports.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CelebrationPassports.Persistence.Configurations;

public class PassportGiftConfiguration : IEntityTypeConfiguration<PassportGift>
{
    public void Configure(EntityTypeBuilder<PassportGift> builder)
    {
        builder.ToTable("PassportGifts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.RecipientName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.RecipientEmail)
            .HasMaxLength(256);

        builder.Property(x => x.GiftMessage)
            .HasMaxLength(1000);

        builder.Property(x => x.MessageMediaUrl)
            .HasMaxLength(500);

        builder.Property(x => x.Amount)
            .HasColumnType("decimal(10,2)");

        builder.Property(x => x.RedemptionCode)
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(x => x.PurchasedOn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(x => x.RedemptionCode).IsUnique();

        // One gift per Passport — a gifted Passport is created solely to be that gift.
        builder.HasIndex(x => x.PassportId).IsUnique();

        builder.HasOne(x => x.Passport)
            .WithOne()
            .HasForeignKey<PassportGift>(x => x.PassportId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.PurchasedByUser)
            .WithMany(u => u.PurchasedPassportGifts)
            .HasForeignKey(x => x.PurchasedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ClaimedByUser)
            .WithMany(u => u.ClaimedPassportGifts)
            .HasForeignKey(x => x.ClaimedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
