using CelebrationPassports.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CelebrationPassports.Persistence.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("Events");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.EventType)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.TimeZoneId)
            .HasMaxLength(100);

        builder.Property(x => x.Notes)
            .HasMaxLength(2000);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

        builder.HasIndex(x => new { x.PassportId, x.StartDate });

        builder.HasOne(x => x.Passport)
            .WithMany(x => x.Events)
            .HasForeignKey(x => x.PassportId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Place)
            .WithMany(x => x.Events)
            .HasForeignKey(x => x.PlaceId)
            .OnDelete(DeleteBehavior.Restrict);

        // Cover-media FKs use SetNull — losing the cover photo shouldn't block deleting it.
        builder.HasOne(x => x.CoverMedia)
            .WithMany()
            .HasForeignKey(x => x.CoverMediaId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.Story)
            .WithMany(x => x.Events)
            .HasForeignKey(x => x.StoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany(x => x.CreatedEvents)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.DeletedByUser)
            .WithMany(x => x.DeletedEvents)
            .HasForeignKey(x => x.DeletedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
