using CelebrationPassports.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CelebrationPassports.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Icon)
            .HasMaxLength(200);

        // Fixed lookup list, seeded once — same role as the Web-side EventType options,
        // but a real FK target since Chapter.CategoryId is required. Fixed GUIDs so the
        // seed is idempotent across environments/migrations.
        builder.HasData(
            new Category { Id = Guid.Parse("9a1b7e10-0001-4a00-8000-000000000001"), Name = "Travel", Icon = "bi-airplane" },
            new Category { Id = Guid.Parse("9a1b7e10-0001-4a00-8000-000000000002"), Name = "Family", Icon = "bi-people" },
            new Category { Id = Guid.Parse("9a1b7e10-0001-4a00-8000-000000000003"), Name = "Celebration", Icon = "bi-balloon-heart" },
            new Category { Id = Guid.Parse("9a1b7e10-0001-4a00-8000-000000000004"), Name = "Food", Icon = "bi-cup-straw" },
            new Category { Id = Guid.Parse("9a1b7e10-0001-4a00-8000-000000000005"), Name = "Friends", Icon = "bi-people-fill" },
            new Category { Id = Guid.Parse("9a1b7e10-0001-4a00-8000-000000000006"), Name = "Milestone", Icon = "bi-trophy" },
            new Category { Id = Guid.Parse("9a1b7e10-0001-4a00-8000-000000000007"), Name = "Adventure", Icon = "bi-compass" },
            new Category { Id = Guid.Parse("9a1b7e10-0001-4a00-8000-000000000008"), Name = "Everyday", Icon = "bi-sun" }
        );
    }
}
