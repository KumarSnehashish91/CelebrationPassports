using CelebrationPassports.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CelebrationPassports.Persistence.Configurations;

public class UserLoginHistoryConfiguration : IEntityTypeConfiguration<UserLoginHistory>
{
    public void Configure(EntityTypeBuilder<UserLoginHistory> builder)
    {
        builder.ToTable("UserLoginHistories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.LoginOn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}