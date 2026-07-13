using CelebrationPassports.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CelebrationPassports.Persistence.Configurations;

public class PassportInvitationConfiguration : IEntityTypeConfiguration<PassportInvitation>
{
    public void Configure(EntityTypeBuilder<PassportInvitation> builder)
    {
        builder.ToTable("PassportInvitations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.HasOne(x => x.Passport)
            .WithMany(x => x.Invitations)
            .HasForeignKey(x => x.PassportId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.InvitedByUser)
            .WithMany(x => x.SentPassportInvitations)
            .HasForeignKey(x => x.InvitedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
