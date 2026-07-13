using CelebrationPassports.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CelebrationPassports.Persistence.Context
{
    public class CelebrationPassportsDbContext : DbContext
    {
        public CelebrationPassportsDbContext(
            DbContextOptions<CelebrationPassportsDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
        public DbSet<UserSession> UserSessions => Set<UserSession>();
        public DbSet<UserLoginHistory> UserLoginHistories => Set<UserLoginHistory>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
        public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Passport> Passports => Set<Passport>();
        public DbSet<PassportPerson> PassportPeople => Set<PassportPerson>();
        public DbSet<PassportInvitation> PassportInvitations => Set<PassportInvitation>();
        public DbSet<PassportShare> PassportShares => Set<PassportShare>();
        public DbSet<PassportOwnershipHistory> PassportOwnershipHistories => Set<PassportOwnershipHistory>();
        public DbSet<Place> Places => Set<Place>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Trip> Trips => Set<Trip>();
        public DbSet<Chapter> Chapters => Set<Chapter>();
       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CelebrationPassportsDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
