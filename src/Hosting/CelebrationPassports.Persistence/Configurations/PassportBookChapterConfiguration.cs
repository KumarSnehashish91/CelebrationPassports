using CelebrationPassports.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CelebrationPassports.Persistence.Configurations;

public class PassportBookChapterConfiguration : IEntityTypeConfiguration<PassportBookChapter>
{
    public void Configure(EntityTypeBuilder<PassportBookChapter> builder)
    {
        builder.ToTable("PassportBookChapters");

        builder.HasKey(x => x.Id);

        // Join row — meaningless without its parent book, always created/removed alongside it.
        builder.HasOne(x => x.Book)
            .WithMany(x => x.BookChapters)
            .HasForeignKey(x => x.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Chapter)
            .WithMany(x => x.BookChapters)
            .HasForeignKey(x => x.ChapterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
