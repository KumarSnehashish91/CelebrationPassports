using CelebrationPassports.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CelebrationPassports.Persistence.Configurations;

public class ImportJobConfiguration : IEntityTypeConfiguration<ImportJob>
{
    public void Configure(EntityTypeBuilder<ImportJob> builder)
    {
        builder.ToTable("ImportJobs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ArchivePath)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.ErrorMessage)
            .HasMaxLength(1000);

        builder.Property(x => x.CreatedOn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(x => x.Passport)
            .WithMany(p => p.ImportJobs)
            .HasForeignKey(x => x.PassportId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany(u => u.CreatedImportJobs)
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.Status, x.CreatedOn });
    }
}
