using CelebrationPassports.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CelebrationPassports.Persistence.Configurations;

public class PassportMilestoneProgressConfiguration : IEntityTypeConfiguration<PassportMilestoneProgress>
{
    public void Configure(EntityTypeBuilder<PassportMilestoneProgress> builder)
    {
        builder.ToTable("PassportMilestoneProgress");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.PassportId, x.MilestoneId })
            .IsUnique();

        builder.HasOne(x => x.Passport)
            .WithMany(x => x.MilestoneProgress)
            .HasForeignKey(x => x.PassportId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Milestone)
            .WithMany(x => x.PassportProgress)
            .HasForeignKey(x => x.MilestoneId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
