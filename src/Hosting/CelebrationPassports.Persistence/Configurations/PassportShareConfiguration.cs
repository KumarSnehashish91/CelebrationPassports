using CelebrationPassports.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CelebrationPassports.Persistence.Configurations;

public class PassportShareConfiguration : IEntityTypeConfiguration<PassportShare>
{
    public void Configure(EntityTypeBuilder<PassportShare> builder)
    {
        builder.ToTable("PassportShares");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ShareToken)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.ViewCount)
            .HasDefaultValue(0);

        builder.HasIndex(x => x.ShareToken)
            .IsUnique();

        builder.HasOne(x => x.Passport)
            .WithMany(x => x.Shares)
            .HasForeignKey(x => x.PassportId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
