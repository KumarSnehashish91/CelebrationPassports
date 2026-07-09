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

    public virtual DbSet<AnalyticsEvent> AnalyticsEvents { get; set; }

    public virtual DbSet<ApplicationLog> ApplicationLogs { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Configuration> Configurations { get; set; }

    public virtual DbSet<DailyStatistic> DailyStatistics { get; set; }

    public virtual DbSet<ErrorLog> ErrorLogs { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<MediaType> MediaTypes { get; set; }

    public virtual DbSet<MomentMedium> MomentMedia { get; set; }

    public virtual DbSet<PageView> PageViews { get; set; }

    public virtual DbSet<Passport> Passports { get; set; }

    public virtual DbSet<PassportInvitation> PassportInvitations { get; set; }

    public virtual DbSet<PassportMember> PassportMembers { get; set; }

    public virtual DbSet<PassportMemberRole> PassportMemberRoles { get; set; }

    public virtual DbSet<PassportMoment> PassportMoments { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SessionLocationInfo> SessionLocationInfos { get; set; }

    public virtual DbSet<SessionTrafficSourceInfo> SessionTrafficSourceInfos { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserDevice> UserDevices { get; set; }

    public virtual DbSet<UserLoginHistory> UserLoginHistories { get; set; }

    public virtual DbSet<UserProfile> UserProfiles { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<UserSession> UserSessions { get; set; }

    public virtual DbSet<UserToken> UserTokens { get; set; }

    public virtual DbSet<Visitor> Visitors { get; set; }

    public virtual DbSet<VisitorSession> VisitorSessions { get; set; }

    public virtual DbSet<VwFeedback> VwFeedbacks { get; set; }

    public virtual DbSet<VwMomentMedium> VwMomentMedia { get; set; }

    public virtual DbSet<VwPageView> VwPageViews { get; set; }

    public virtual DbSet<VwPassport> VwPassports { get; set; }

    public virtual DbSet<VwPassportMoment> VwPassportMoments { get; set; }

    public virtual DbSet<VwUser> VwUsers { get; set; }

    public virtual DbSet<VwVisitor> VwVisitors { get; set; }

    public virtual DbSet<VwVisitorSession> VwVisitorSessions { get; set; }

    public virtual DbSet<VwWaitlist> VwWaitlists { get; set; }

    public virtual DbSet<Waitlist> Waitlists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnalyticsEvent>(entity =>
        {
            entity.Property(e => e.AnalyticsEventId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())", "DF_AnalyticsEvents_CreatedOn");
            entity.Property(e => e.ElementId).HasMaxLength(200);
            entity.Property(e => e.ElementText).HasMaxLength(500);
            entity.Property(e => e.EntityName).HasMaxLength(100);
            entity.Property(e => e.EventCategory).HasMaxLength(100);
            entity.Property(e => e.EventName).HasMaxLength(150);
            entity.Property(e => e.EventOccurredOn).HasDefaultValueSql("(sysutcdatetime())", "DF_AnalyticsEvents_EventOccurredOn");
            entity.Property(e => e.EventValue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PageUrl).HasMaxLength(1000);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.User).WithMany(p => p.AnalyticsEvents)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_AnalyticsEvents_Users");

            entity.HasOne(d => d.Visitor).WithMany(p => p.AnalyticsEvents)
                .HasForeignKey(d => d.VisitorId)
                .HasConstraintName("FK_AnalyticsEvents_Visitors");

            entity.HasOne(d => d.VisitorSession).WithMany(p => p.AnalyticsEvents)
                .HasForeignKey(d => d.VisitorSessionId)
                .HasConstraintName("FK_AnalyticsEvents_VisitorSessions");
        });

        modelBuilder.Entity<ApplicationLog>(entity =>
        {
            entity.HasKey(e => e.ApplicationLogId).HasName("PK__Applicat__00948F41D95A6665");

            entity.Property(e => e.ApplicationLogId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.HttpMethod).HasMaxLength(20);
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(50)
                .HasColumnName("IPAddress");
            entity.Property(e => e.LogLevel).HasMaxLength(20);
            entity.Property(e => e.RequestPath).HasMaxLength(500);
            entity.Property(e => e.Source).HasMaxLength(200);
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditLogId).HasName("PK__AuditLog__EB5F6CBDD44007AB");

            entity.Property(e => e.AuditLogId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Action).HasMaxLength(100);
            entity.Property(e => e.Browser).HasMaxLength(100);
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.EntityName).HasMaxLength(100);
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(50)
                .HasColumnName("IPAddress");
            entity.Property(e => e.Module).HasMaxLength(100);
        });

        modelBuilder.Entity<Configuration>(entity =>
        {
            entity.HasIndex(e => e.Category, "IX_Configurations_Category");

            entity.HasIndex(e => e.ConfigurationKey, "IX_Configurations_Key");

            entity.HasIndex(e => e.ConfigurationKey, "UQ_Configurations_Key").IsUnique();

            entity.Property(e => e.ConfigurationId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.ConfigurationKey).HasMaxLength(150);
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.DataType).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.DisplayOrder).HasDefaultValue(1, "DF_Configurations_DisplayOrder");
            entity.Property(e => e.IsEditable).HasDefaultValue(true, "DF_Configurations_IsEditable");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
        });

        modelBuilder.Entity<DailyStatistic>(entity =>
        {
            entity.HasKey(e => e.DailyStatisticsId).HasName("PK__DailySta__2DCF31E20B7C09BA");

            entity.Property(e => e.DailyStatisticsId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.AverageRating).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.AverageSessionDuration).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.BounceRate).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.FeedbackReceived).HasDefaultValue(0);
            entity.Property(e => e.MediaUploaded).HasDefaultValue(0);
            entity.Property(e => e.MomentsCreated).HasDefaultValue(0);
            entity.Property(e => e.PassportsCreated).HasDefaultValue(0);
            entity.Property(e => e.ReturningVisitors).HasDefaultValue(0);
            entity.Property(e => e.UniqueVisitors).HasDefaultValue(0);
            entity.Property(e => e.UserRegistrations).HasDefaultValue(0);
            entity.Property(e => e.Visitors).HasDefaultValue(0);
            entity.Property(e => e.WaitlistRegistrations).HasDefaultValue(0);
        });

        modelBuilder.Entity<ErrorLog>(entity =>
        {
            entity.HasKey(e => e.ErrorLogId).HasName("PK__ErrorLog__D65247C229A1F45B");

            entity.Property(e => e.ErrorLogId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Api)
                .HasMaxLength(300)
                .HasColumnName("API");
            entity.Property(e => e.Browser).HasMaxLength(100);
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.ExceptionType).HasMaxLength(200);
            entity.Property(e => e.HttpMethod).HasMaxLength(20);
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(50)
                .HasColumnName("IPAddress");
            entity.Property(e => e.Source).HasMaxLength(200);
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.ToTable("Feedback");

            entity.HasIndex(e => e.CreatedOn, "IX_Feedback_CreatedOn");

            entity.HasIndex(e => e.Rating, "IX_Feedback_Rating");

            entity.HasIndex(e => e.Status, "IX_Feedback_Status");

            entity.HasIndex(e => e.UserId, "IX_Feedback_UserId");

            entity.Property(e => e.FeedbackId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())", "DF_Feedback_CreatedOn");
            entity.Property(e => e.FavouriteFeature).HasMaxLength(300);
            entity.Property(e => e.FeedbackSource)
                .HasMaxLength(50)
                .HasDefaultValue("Website", "DF_Feedback_FeedbackSource");
            entity.Property(e => e.MissingFeature).HasMaxLength(500);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("Pending", "DF_Feedback_Status");

            entity.HasOne(d => d.User).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Feedback_Users");

            entity.HasOne(d => d.Visitor).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.VisitorId)
                .HasConstraintName("FK_Feedback_Visitors");

            entity.HasOne(d => d.VisitorSession).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.VisitorSessionId)
                .HasConstraintName("FK_Feedback_VisitorSessions");

            entity.HasOne(d => d.Waitlist).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.WaitlistId)
                .HasConstraintName("FK_Feedback_Waitlist");
        });

        modelBuilder.Entity<MediaType>(entity =>
        {
            entity.HasIndex(e => e.Code, "UQ_MediaTypes_Code").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<MomentMedium>(entity =>
        {
            entity.HasIndex(e => e.PassportMomentId, "IX_MomentMedia_PassportMomentId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())", "DF_MomentMedia_Id");
            entity.Property(e => e.Caption).HasMaxLength(500);
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.FileName).HasMaxLength(500);
            entity.Property(e => e.FilePath).HasMaxLength(1000);
            entity.Property(e => e.MimeType).HasMaxLength(200);

            entity.HasOne(d => d.MediaType).WithMany(p => p.MomentMedia)
                .HasForeignKey(d => d.MediaTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MomentMedia_MediaTypes");

            entity.HasOne(d => d.PassportMoment).WithMany(p => p.MomentMedia)
                .HasForeignKey(d => d.PassportMomentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MomentMedia_PassportMoments");
        });

        modelBuilder.Entity<PageView>(entity =>
        {
            entity.HasIndex(e => e.EnteredOn, "IX_PageViews_EnteredOn");

            entity.HasIndex(e => e.PageUrl, "IX_PageViews_PageUrl");

            entity.HasIndex(e => e.VisitorSessionId, "IX_PageViews_VisitorSessionId");

            entity.Property(e => e.PageViewId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())", "DF_PageViews_CreatedOn");
            entity.Property(e => e.EnteredOn).HasDefaultValueSql("(sysutcdatetime())", "DF_PageViews_EnteredOn");
            entity.Property(e => e.PageTitle).HasMaxLength(300);
            entity.Property(e => e.PageUrl).HasMaxLength(1000);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.ScrollPercentage).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.VisitorSession).WithMany(p => p.PageViews)
                .HasForeignKey(d => d.VisitorSessionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PageViews_VisitorSessions");
        });

        modelBuilder.Entity<Passport>(entity =>
        {
            entity.HasIndex(e => e.CreatedBy, "IX_Passports_CreatedBy");

            entity.HasIndex(e => e.StatusId, "IX_Passports_StatusId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())", "DF_Passports_Id");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())", "DF_Passports_CreatedOn");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.CoverMedia).WithMany(p => p.Passports)
                .HasForeignKey(d => d.CoverMediaId)
                .HasConstraintName("FK_Passports_CoverMedia");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PassportCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Passports_CreatedBy");

            entity.HasOne(d => d.DeletedByNavigation).WithMany(p => p.PassportDeletedByNavigations)
                .HasForeignKey(d => d.DeletedBy)
                .HasConstraintName("FK_Passports_DeletedBy");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.PassportModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("FK_Passports_ModifedBy");

            entity.HasOne(d => d.Status).WithMany(p => p.Passports)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Passports_Statuses");
        });

        modelBuilder.Entity<PassportInvitation>(entity =>
        {
            entity.HasIndex(e => e.Email, "IX_PassportInvitations_Email");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())", "DF_PassportInvitations_Id");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Email).HasMaxLength(320);
            entity.Property(e => e.InvitationToken).HasDefaultValueSql("(newsequentialid())");

            entity.HasOne(d => d.Passport).WithMany(p => p.PassportInvitations)
                .HasForeignKey(d => d.PassportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PassportInvitations_Passports");

            entity.HasOne(d => d.PassportMemberRole).WithMany(p => p.PassportInvitations)
                .HasForeignKey(d => d.PassportMemberRoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PassportInvitations_Roles");
        });

        modelBuilder.Entity<PassportMember>(entity =>
        {
            entity.HasKey(e => new { e.PassportId, e.UserId });

            entity.HasIndex(e => e.UserId, "IX_PassportMembers_UserId");

            entity.Property(e => e.JoinedOn).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Passport).WithMany(p => p.PassportMembers)
                .HasForeignKey(d => d.PassportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PassportMembers_Passports");

            entity.HasOne(d => d.PassportMemberRole).WithMany(p => p.PassportMembers)
                .HasForeignKey(d => d.PassportMemberRoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PassportMembers_PassportMemberRoles");

            entity.HasOne(d => d.User).WithMany(p => p.PassportMembers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PassportMembers_Users");
        });

        modelBuilder.Entity<PassportMemberRole>(entity =>
        {
            entity.HasIndex(e => e.Code, "UQ_PassportMemberRoles_Code").IsUnique();

            entity.HasIndex(e => e.Name, "UQ_PassportMemberRoles_Name").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsSystem).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
        });

        modelBuilder.Entity<PassportMoment>(entity =>
        {
            entity.HasIndex(e => e.PassportId, "IX_PassportMoments_PassportId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())", "DF_PassportMoments_Id");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())", "DF_PassportMoments_CreatedOn");
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Location).HasMaxLength(500);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.CoverMedia).WithMany(p => p.PassportMoments)
                .HasForeignKey(d => d.CoverMediaId)
                .HasConstraintName("FK_PassportMoments_CoverMedia");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PassportMomentCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PassportMoments_CreatedBy");

            entity.HasOne(d => d.DeletedByNavigation).WithMany(p => p.PassportMomentDeletedByNavigations)
                .HasForeignKey(d => d.DeletedBy)
                .HasConstraintName("FK_PassportMoments_DeletedBy");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.PassportMomentModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("FK_PassportMoments_ModifiedBy");

            entity.HasOne(d => d.Passport).WithMany(p => p.PassportMoments)
                .HasForeignKey(d => d.PassportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PassportMoments_Passports");

            entity.HasOne(d => d.Status).WithMany(p => p.PassportMoments)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PassportMoments_Statuses");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasIndex(e => e.Code, "UQ_Permissions_Code").IsUnique();

            entity.HasIndex(e => e.Name, "UQ_Permissions_Name").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(100);
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsSystem).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_RefreshTokens_UserId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())", "DF_RefreshTokens_Id");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())", "DF_RefreshTokens_CreatedOn");
            entity.Property(e => e.TokenHash).HasMaxLength(500);

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RefreshTokens_Users");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(e => e.Code, "UQ_Roles_Code").IsUnique();

            entity.HasIndex(e => e.Name, "UQ_Roles_Name").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsSystem).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
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

        modelBuilder.Entity<SessionLocationInfo>(entity =>
        {
            entity.ToTable("SessionLocationInfo");

            entity.HasIndex(e => e.City, "IX_SessionLocationInfo_City");

            entity.HasIndex(e => e.Country, "IX_SessionLocationInfo_Country");

            entity.HasIndex(e => e.Ipaddress, "IX_SessionLocationInfo_IPAddress");

            entity.HasIndex(e => e.State, "IX_SessionLocationInfo_State");

            entity.HasIndex(e => e.VisitorSessionId, "UQ_SessionLocationInfo_VisitorSessionId").IsUnique();

            entity.Property(e => e.SessionLocationInfoId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Asn)
                .HasMaxLength(50)
                .HasColumnName("ASN");
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.ConnectionType).HasMaxLength(50);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.CountryCode)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())", "DF_SessionLocationInfo_CreatedOn");
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(50)
                .HasColumnName("IPAddress");
            entity.Property(e => e.IsVpn).HasColumnName("IsVPN");
            entity.Property(e => e.Isp)
                .HasMaxLength(200)
                .HasColumnName("ISP");
            entity.Property(e => e.Latitude).HasColumnType("decimal(10, 7)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(10, 7)");
            entity.Property(e => e.Organization).HasMaxLength(200);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.TimeZone).HasMaxLength(100);

            entity.HasOne(d => d.VisitorSession).WithOne(p => p.SessionLocationInfo)
                .HasForeignKey<SessionLocationInfo>(d => d.VisitorSessionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SessionLocationInfo_VisitorSessions");
        });

        modelBuilder.Entity<SessionTrafficSourceInfo>(entity =>
        {
            entity.HasKey(e => e.SessionTrafficSourceInfoId).HasName("PK__SessionT__1BC66BA4A5852941");

            entity.ToTable("SessionTrafficSourceInfo");

            entity.Property(e => e.SessionTrafficSourceInfoId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Campaign).HasMaxLength(200);
            entity.Property(e => e.Content).HasMaxLength(200);
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Medium).HasMaxLength(100);
            entity.Property(e => e.Referrer).HasMaxLength(1000);
            entity.Property(e => e.Source).HasMaxLength(100);
            entity.Property(e => e.Term).HasMaxLength(200);
            entity.Property(e => e.Utmcampaign)
                .HasMaxLength(200)
                .HasColumnName("UTMCampaign");
            entity.Property(e => e.Utmcontent)
                .HasMaxLength(200)
                .HasColumnName("UTMContent");
            entity.Property(e => e.Utmmedium)
                .HasMaxLength(200)
                .HasColumnName("UTMMedium");
            entity.Property(e => e.Utmsource)
                .HasMaxLength(200)
                .HasColumnName("UTMSource");
            entity.Property(e => e.Utmterm)
                .HasMaxLength(200)
                .HasColumnName("UTMTerm");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasIndex(e => e.Code, "UQ_Statuses_Code").IsUnique();

            entity.HasIndex(e => e.Name, "UQ_Statuses_Name").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsSystem).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.StatusId, "IX_Users_StatusId");

            entity.HasIndex(e => e.Email, "UQ_Users_Email").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())", "DF_Users_Id");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())", "DF_Users_CreatedOn");
            entity.Property(e => e.Email).HasMaxLength(320);
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash).HasMaxLength(500);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Status).WithMany(p => p.Users)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Statuses");
        });

        modelBuilder.Entity<UserDevice>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_UserDevices_UserId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())", "DF_UserDevices_Id");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())", "DF_UserDevices_CreatedOn");
            entity.Property(e => e.DeviceIdentifier).HasMaxLength(200);
            entity.Property(e => e.DeviceName).HasMaxLength(200);
            entity.Property(e => e.DeviceType).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_UserDevices_IsActive");

            entity.HasOne(d => d.User).WithMany(p => p.UserDevices)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserDevices_Users");
        });

        modelBuilder.Entity<UserLoginHistory>(entity =>
        {
            entity.ToTable("UserLoginHistory");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())", "DF_UserLoginHistory_Id");
            entity.Property(e => e.FailureReason).HasMaxLength(1000);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.LoginOn).HasDefaultValueSql("(sysutcdatetime())", "DF_UserLoginHistory_LoginOn");
            entity.Property(e => e.UserAgent).HasMaxLength(1000);

            entity.HasOne(d => d.User).WithMany(p => p.UserLoginHistories)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserLoginHistory_Users");
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())", "DF_UserProfiles_CreatedOn");
            entity.Property(e => e.DisplayName).HasMaxLength(200);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.MobileNumber).HasMaxLength(20);
            entity.Property(e => e.ProfilePhotoUrl).HasMaxLength(500);

            entity.HasOne(d => d.User).WithOne(p => p.UserProfile)
                .HasForeignKey<UserProfile>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserProfiles_Users");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId });

            entity.HasIndex(e => e.RoleId, "IX_UserRoles_RoleId");

            entity.Property(e => e.AssignedOn).HasDefaultValueSql("(sysutcdatetime())", "DF_UserRoles_AssignedOn");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRoles_Roles");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRoles_Users");
        });

        modelBuilder.Entity<UserSession>(entity =>
        {
            entity.HasIndex(e => e.IsActive, "IX_UserSessions_IsActive");

            entity.HasIndex(e => e.RefreshToken, "IX_UserSessions_RefreshToken");

            entity.HasIndex(e => e.UserId, "IX_UserSessions_UserId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Browser).HasMaxLength(200);
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())", "DF_UserSessions_CreatedOn");
            entity.Property(e => e.DeviceName).HasMaxLength(200);
            entity.Property(e => e.DeviceType).HasMaxLength(50);
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(50)
                .HasColumnName("IPAddress");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_UserSessions_IsActive");
            entity.Property(e => e.LoggedInOn).HasDefaultValueSql("(sysutcdatetime())", "DF_UserSessions_LoggedInOn");
            entity.Property(e => e.OperatingSystem).HasMaxLength(200);
            entity.Property(e => e.RefreshToken).HasMaxLength(500);
            entity.Property(e => e.RevokedReason).HasMaxLength(250);
            entity.Property(e => e.UserAgent).HasMaxLength(1000);

            entity.HasOne(d => d.User).WithMany(p => p.UserSessions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserSessions_Users");
        });

        modelBuilder.Entity<UserToken>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_UserTokens_UserId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())", "DF_UserTokens_Id");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())", "DF_UserTokens_CreatedOn");
            entity.Property(e => e.Token).HasMaxLength(500);
            entity.Property(e => e.TokenType).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.UserTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserTokens_Users");
        });

        modelBuilder.Entity<Visitor>(entity =>
        {
            entity.HasIndex(e => e.FirstVisitOn, "IX_Visitors_FirstVisitOn");

            entity.HasIndex(e => e.IsBetaInterested, "IX_Visitors_IsBetaInterested");

            entity.HasIndex(e => e.IsRegistered, "IX_Visitors_IsRegistered");

            entity.HasIndex(e => e.LastVisitOn, "IX_Visitors_LastVisitOn");

            entity.HasIndex(e => e.AnonymousId, "UQ_Visitors_AnonymousId").IsUnique();

            entity.Property(e => e.VisitorId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.AnonymousId).HasMaxLength(150);
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())", "DF_Visitors_CreatedOn");
            entity.Property(e => e.FirstVisitOn).HasDefaultValueSql("(sysutcdatetime())", "DF_Visitors_FirstVisitOn");
            entity.Property(e => e.LastVisitOn).HasDefaultValueSql("(sysutcdatetime())", "DF_Visitors_LastVisitOn");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.TotalSessions).HasDefaultValue(1, "DF_Visitors_TotalSessions");
            entity.Property(e => e.TotalVisits).HasDefaultValue(1, "DF_Visitors_TotalVisits");

            entity.HasOne(d => d.RegisteredUser).WithMany(p => p.Visitors)
                .HasForeignKey(d => d.RegisteredUserId)
                .HasConstraintName("FK_Visitors_Users");
        });

        modelBuilder.Entity<VisitorSession>(entity =>
        {
            entity.HasIndex(e => e.LastActivityOn, "IX_VisitorSessions_LastActivityOn");

            entity.HasIndex(e => e.SessionStartOn, "IX_VisitorSessions_SessionStartOn");

            entity.HasIndex(e => e.TrailerPlayed, "IX_VisitorSessions_TrailerPlayed");

            entity.HasIndex(e => e.VisitorId, "IX_VisitorSessions_VisitorId");

            entity.HasIndex(e => e.SessionIdentifier, "UQ_VisitorSessions_SessionIdentifier").IsUnique();

            entity.Property(e => e.VisitorSessionId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())", "DF_VisitorSessions_CreatedOn");
            entity.Property(e => e.ExitPage).HasMaxLength(500);
            entity.Property(e => e.LandingPage).HasMaxLength(500);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.SessionIdentifier).HasMaxLength(150);
            entity.Property(e => e.SessionStartOn).HasDefaultValueSql("(sysutcdatetime())", "DF_VisitorSessions_SessionStartOn");
            entity.Property(e => e.TotalPageViews).HasDefaultValue(1, "DF_VisitorSessions_TotalPageViews");

            entity.HasOne(d => d.Visitor).WithMany(p => p.VisitorSessions)
                .HasForeignKey(d => d.VisitorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VisitorSessions_Visitors");
        });

        modelBuilder.Entity<VwFeedback>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwFeedback");

            entity.Property(e => e.Email).HasMaxLength(320);
            entity.Property(e => e.FavouriteFeature).HasMaxLength(300);
            entity.Property(e => e.FeedbackSource).HasMaxLength(50);
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.MissingFeature).HasMaxLength(500);
            entity.Property(e => e.Status).HasMaxLength(30);
        });

        modelBuilder.Entity<VwMomentMedium>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwMomentMedia");

            entity.Property(e => e.Caption).HasMaxLength(500);
            entity.Property(e => e.FileName).HasMaxLength(500);
            entity.Property(e => e.FilePath).HasMaxLength(1000);
            entity.Property(e => e.MediaType).HasMaxLength(50);
            entity.Property(e => e.MimeType).HasMaxLength(200);
            entity.Property(e => e.MomentTitle).HasMaxLength(200);
        });

        modelBuilder.Entity<VwPageView>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwPageViews");

            entity.Property(e => e.PageTitle).HasMaxLength(300);
            entity.Property(e => e.PageUrl).HasMaxLength(1000);
            entity.Property(e => e.ScrollPercentage).HasColumnType("decimal(5, 2)");
        });

        modelBuilder.Entity<VwPassport>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwPassports");

            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Title).HasMaxLength(200);
        });

        modelBuilder.Entity<VwPassportMoment>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwPassportMoments");

            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Location).HasMaxLength(500);
            entity.Property(e => e.PassportTitle).HasMaxLength(200);
            entity.Property(e => e.Title).HasMaxLength(200);
        });

        modelBuilder.Entity<VwUser>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwUsers");

            entity.Property(e => e.DisplayName).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(320);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.MobileNumber).HasMaxLength(20);
        });

        modelBuilder.Entity<VwVisitor>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwVisitors");

            entity.Property(e => e.AnonymousId).HasMaxLength(150);
            entity.Property(e => e.EmailAddress).HasMaxLength(320);
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.InterestedPlatform).HasMaxLength(30);
            entity.Property(e => e.InvitationStatus).HasMaxLength(30);
            entity.Property(e => e.MobileNumber).HasMaxLength(20);
            entity.Property(e => e.RegisteredEmail).HasMaxLength(320);
        });

        modelBuilder.Entity<VwVisitorSession>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwVisitorSessions");

            entity.Property(e => e.AnonymousId).HasMaxLength(150);
            entity.Property(e => e.Campaign).HasMaxLength(200);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.CountryCode)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ExitPage).HasMaxLength(500);
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(50)
                .HasColumnName("IPAddress");
            entity.Property(e => e.LandingPage).HasMaxLength(500);
            entity.Property(e => e.Medium).HasMaxLength(100);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.Referrer).HasMaxLength(1000);
            entity.Property(e => e.SessionIdentifier).HasMaxLength(150);
            entity.Property(e => e.Source).HasMaxLength(100);
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.TimeZone).HasMaxLength(100);
        });

        modelBuilder.Entity<VwWaitlist>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwWaitlist");

            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.CompanyName).HasMaxLength(200);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.EmailAddress).HasMaxLength(320);
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.InterestedPlatform).HasMaxLength(30);
            entity.Property(e => e.InvitationBatch).HasMaxLength(50);
            entity.Property(e => e.InvitationStatus).HasMaxLength(30);
            entity.Property(e => e.MobileNumber).HasMaxLength(20);
            entity.Property(e => e.Occupation).HasMaxLength(150);
            entity.Property(e => e.ReferralSource).HasMaxLength(100);
            entity.Property(e => e.RegisteredEmail).HasMaxLength(320);
            entity.Property(e => e.State).HasMaxLength(100);
        });

        modelBuilder.Entity<Waitlist>(entity =>
        {
            entity.ToTable("Waitlist");

            entity.HasIndex(e => e.CreatedOn, "IX_Waitlist_CreatedOn");

            entity.HasIndex(e => e.EmailAddress, "IX_Waitlist_EmailAddress");

            entity.HasIndex(e => e.InvitationStatus, "IX_Waitlist_InvitationStatus");

            entity.HasIndex(e => e.EmailAddress, "UQ_Waitlist_EmailAddress").IsUnique();

            entity.Property(e => e.WaitlistId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.CompanyName).HasMaxLength(200);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())", "DF_Waitlist_CreatedOn");
            entity.Property(e => e.EmailAddress).HasMaxLength(320);
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.InterestedPlatform).HasMaxLength(30);
            entity.Property(e => e.InvitationBatch).HasMaxLength(50);
            entity.Property(e => e.InvitationStatus)
                .HasMaxLength(30)
                .HasDefaultValue("Pending", "DF_Waitlist_InvitationStatus");
            entity.Property(e => e.MobileNumber).HasMaxLength(20);
            entity.Property(e => e.Occupation).HasMaxLength(150);
            entity.Property(e => e.ReferralSource).HasMaxLength(100);
            entity.Property(e => e.Remarks).HasMaxLength(1000);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.WantsBetaAccess).HasDefaultValue(true, "DF_Waitlist_WantsBetaAccess");
            entity.Property(e => e.WantsNewsletter).HasDefaultValue(true, "DF_Waitlist_WantsNewsletter");

            entity.HasOne(d => d.User).WithMany(p => p.Waitlists)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Waitlist_Users");

            entity.HasOne(d => d.Visitor).WithMany(p => p.Waitlists)
                .HasForeignKey(d => d.VisitorId)
                .HasConstraintName("FK_Waitlist_Visitors");

            entity.HasOne(d => d.VisitorSession).WithMany(p => p.Waitlists)
                .HasForeignKey(d => d.VisitorSessionId)
                .HasConstraintName("FK_Waitlist_VisitorSessions");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
