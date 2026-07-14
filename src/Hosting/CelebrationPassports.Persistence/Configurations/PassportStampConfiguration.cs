using CelebrationPassports.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CelebrationPassports.Persistence.Configurations;

public class PassportStampConfiguration : IEntityTypeConfiguration<PassportStamp>
{
    public void Configure(EntityTypeBuilder<PassportStamp> builder)
    {
        builder.ToTable("PassportStamps");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Passport)
            .WithMany(x => x.PassportStamps)
            .HasForeignKey(x => x.PassportId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Place)
            .WithMany(x => x.PassportStamps)
            .HasForeignKey(x => x.PlaceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SourceChapter)
            .WithMany(x => x.PassportStamps)
            .HasForeignKey(x => x.SourceChapterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
