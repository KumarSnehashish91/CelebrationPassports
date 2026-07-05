using System;
using System.Collections.Generic;
using CelebrationPassports.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace CelebrationPassports.Persistence.Context;

public partial class CelebrationPassportsDbContext : DbContext
{
    public CelebrationPassportsDbContext(DbContextOptions<CelebrationPassportsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<MediaType> MediaTypes { get; set; }

    public virtual DbSet<MomentMedium> MomentMedia { get; set; }

    public virtual DbSet<Passport> Passports { get; set; }

    public virtual DbSet<PassportInvitation> PassportInvitations { get; set; }

    public virtual DbSet<PassportMember> PassportMembers { get; set; }

    public virtual DbSet<PassportMemberRole> PassportMemberRoles { get; set; }

    public virtual DbSet<PassportMoment> PassportMoments { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserDevice> UserDevices { get; set; }

    public virtual DbSet<UserLoginHistory> UserLoginHistories { get; set; }

    public virtual DbSet<UserProfile> UserProfiles { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<UserToken> UserTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MediaType>(entity =>
        {
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<MomentMedium>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())", "DF_MomentMedia_Id");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.MediaType).WithMany(p => p.MomentMedia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MomentMedia_MediaTypes");

            entity.HasOne(d => d.PassportMoment).WithMany(p => p.MomentMedia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MomentMedia_PassportMoments");
        });

        modelBuilder.Entity<Passport>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())", "DF_Passports_Id");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())", "DF_Passports_CreatedOn");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CoverMedia).WithMany(p => p.Passports).HasConstraintName("FK_Passports_CoverMedia");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PassportCreatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Passports_CreatedBy");

            entity.HasOne(d => d.DeletedByNavigation).WithMany(p => p.PassportDeletedByNavigations).HasConstraintName("FK_Passports_DeletedBy");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.PassportModifiedByNavigations).HasConstraintName("FK_Passports_ModifedBy");

            entity.HasOne(d => d.Status).WithMany(p => p.Passports)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Passports_Statuses");
        });

        modelBuilder.Entity<PassportInvitation>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())", "DF_PassportInvitations_Id");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.InvitationToken).HasDefaultValueSql("(newsequentialid())");

            entity.HasOne(d => d.Passport).WithMany(p => p.PassportInvitations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PassportInvitations_Passports");

            entity.HasOne(d => d.PassportMemberRole).WithMany(p => p.PassportInvitations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PassportInvitations_Roles");
        });

        modelBuilder.Entity<PassportMember>(entity =>
        {
            entity.Property(e => e.JoinedOn).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Passport).WithMany(p => p.PassportMembers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PassportMembers_Passports");

            entity.HasOne(d => d.PassportMemberRole).WithMany(p => p.PassportMembers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PassportMembers_PassportMemberRoles");

            entity.HasOne(d => d.User).WithMany(p => p.PassportMembers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PassportMembers_Users");
        });

        modelBuilder.Entity<PassportMemberRole>(entity =>
        {
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsSystem).HasDefaultValue(true);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
        });

        modelBuilder.Entity<PassportMoment>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())", "DF_PassportMoments_Id");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())", "DF_PassportMoments_CreatedOn");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CoverMedia).WithMany(p => p.PassportMoments).HasConstraintName("FK_PassportMoments_CoverMedia");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PassportMomentCreatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PassportMoments_CreatedBy");

            entity.HasOne(d => d.DeletedByNavigation).WithMany(p => p.PassportMomentDeletedByNavigations).HasConstraintName("FK_PassportMoments_DeletedBy");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.PassportMomentModifiedByNavigations).HasConstraintName("FK_PassportMoments_ModifiedBy");

            entity.HasOne(d => d.Passport).WithMany(p => p.PassportMoments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PassportMoments_Passports");

            entity.HasOne(d => d.Status).WithMany(p => p.PassportMoments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PassportMoments_Statuses");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsSystem).HasDefaultValue(true);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())", "DF_RefreshTokens_Id");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())", "DF_RefreshTokens_CreatedOn");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RefreshTokens_Users");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsSystem).HasDefaultValue(true);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasMany(d => d.Permissions).WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "RolePermission",
                    r => r.HasOne<Permission>().WithMany()
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_RolePermissions_Permissions"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_RolePermissions_Roles"),
                    j =>
                    {
                        j.HasKey("RoleId", "PermissionId");
                        j.ToTable("RolePermissions");
                        j.HasIndex(new[] { "PermissionId" }, "IX_RolePermissions_PermissionId");
                    });
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsSystem).HasDefaultValue(true);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())", "DF_Users_Id");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())", "DF_Users_CreatedOn");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Status).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Statuses");
        });

        modelBuilder.Entity<UserDevice>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())", "DF_UserDevices_Id");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())", "DF_UserDevices_CreatedOn");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_UserDevices_IsActive");

            entity.HasOne(d => d.User).WithMany(p => p.UserDevices)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserDevices_Users");
        });

        modelBuilder.Entity<UserLoginHistory>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())", "DF_UserLoginHistory_Id");
            entity.Property(e => e.LoginOn).HasDefaultValueSql("(sysutcdatetime())", "DF_UserLoginHistory_LoginOn");

            entity.HasOne(d => d.User).WithMany(p => p.UserLoginHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserLoginHistory_Users");
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())", "DF_UserProfiles_CreatedOn");
            entity.Property(e => e.Gender).IsFixedLength();

            entity.HasOne(d => d.User).WithOne(p => p.UserProfile)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserProfiles_Users");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.Property(e => e.AssignedOn).HasDefaultValueSql("(sysutcdatetime())", "DF_UserRoles_AssignedOn");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRoles_Roles");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRoles_Users");
        });

        modelBuilder.Entity<UserToken>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())", "DF_UserTokens_Id");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())", "DF_UserTokens_CreatedOn");

            entity.HasOne(d => d.User).WithMany(p => p.UserTokens)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserTokens_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
