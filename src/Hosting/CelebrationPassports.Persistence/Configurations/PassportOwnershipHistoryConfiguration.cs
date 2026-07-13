using CelebrationPassports.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CelebrationPassports.Persistence.Configurations;

public class PassportOwnershipHistoryConfiguration : IEntityTypeConfiguration<PassportOwnershipHistory>
{
    public void Configure(EntityTypeBuilder<PassportOwnershipHistory> builder)
    {
        builder.ToTable("PassportOwnershipHistory");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TransferredOn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(x => x.Reason)
            .HasMaxLength(1000);

        builder.HasOne(x => x.Passport)
            .WithMany(x => x.OwnershipHistory)
            .HasForeignKey(x => x.PassportId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.OldOwner)
            .WithMany(x => x.OwnershipTransfersFrom)
            .HasForeignKey(x => x.OldOwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.NewOwner)
            .WithMany(x => x.OwnershipTransfersTo)
            .HasForeignKey(x => x.NewOwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TransferredByUser)
            .WithMany(x => x.OwnershipTransfersPerformed)
            .HasForeignKey(x => x.TransferredBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
