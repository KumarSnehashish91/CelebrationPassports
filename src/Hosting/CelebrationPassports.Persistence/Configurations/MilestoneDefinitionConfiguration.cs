using CelebrationPassports.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CelebrationPassports.Persistence.Configurations;

public class MilestoneDefinitionConfiguration : IEntityTypeConfiguration<MilestoneDefinition>
{
    public void Configure(EntityTypeBuilder<MilestoneDefinition> builder)
    {
        builder.ToTable("MilestoneDefinitions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.MetricType)
            .IsRequired();
    }
}
