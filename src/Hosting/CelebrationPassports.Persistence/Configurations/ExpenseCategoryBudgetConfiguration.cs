using CelebrationPassports.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CelebrationPassports.Persistence.Configurations;

public class ExpenseCategoryBudgetConfiguration : IEntityTypeConfiguration<ExpenseCategoryBudget>
{
    public void Configure(EntityTypeBuilder<ExpenseCategoryBudget> builder)
    {
        builder.ToTable("ExpenseCategoryBudgets");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.BudgetedAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(x => x.Event)
            .WithMany()
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.EventId, x.Category })
            .IsUnique();
    }
}
