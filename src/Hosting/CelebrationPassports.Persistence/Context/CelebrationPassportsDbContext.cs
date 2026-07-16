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
        public DbSet<Event> Events => Set<Event>();
        public DbSet<CalendarEvent> CalendarEvents => Set<CalendarEvent>();
        public DbSet<Story> Stories => Set<Story>();
        public DbSet<Chapter> Chapters => Set<Chapter>();
        public DbSet<Media> Media => Set<Media>();
        public DbSet<MediaVariant> MediaVariants => Set<MediaVariant>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Reaction> Reactions => Set<Reaction>();
        public DbSet<WishlistItem> WishlistItems => Set<WishlistItem>();
        public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
        public DbSet<PassportBook> PassportBooks => Set<PassportBook>();
        public DbSet<PassportBookChapter> PassportBookChapters => Set<PassportBookChapter>();
        public DbSet<PassportStamp> PassportStamps => Set<PassportStamp>();
        public DbSet<MilestoneDefinition> MilestoneDefinitions => Set<MilestoneDefinition>();
        public DbSet<PassportMilestoneProgress> PassportMilestoneProgress => Set<PassportMilestoneProgress>();
        public DbSet<SomedayIdea> SomedayIdeas => Set<SomedayIdea>();
        public DbSet<TimeCapsuleMessage> TimeCapsuleMessages => Set<TimeCapsuleMessage>();
        public DbSet<ImportJob> ImportJobs => Set<ImportJob>();
        public DbSet<PassportGift> PassportGifts => Set<PassportGift>();
        public DbSet<GiftDraft> GiftDrafts => Set<GiftDraft>();
        public DbSet<GiftPhoto> GiftPhotos => Set<GiftPhoto>();
        public DbSet<GeneratedStory> GeneratedStories => Set<GeneratedStory>();
        public DbSet<Expense> Expenses => Set<Expense>();
        public DbSet<ExpenseCategoryBudget> ExpenseCategoryBudgets => Set<ExpenseCategoryBudget>();
        public DbSet<TripItineraryDay> TripItineraryDays => Set<TripItineraryDay>();
        public DbSet<GuestbookSubmission> GuestbookSubmissions => Set<GuestbookSubmission>();
        public DbSet<ChapterInvitation> ChapterInvitations => Set<ChapterInvitation>();
        public DbSet<ChapterContributor> ChapterContributors => Set<ChapterContributor>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CelebrationPassportsDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
