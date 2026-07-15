using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Dashboard;
using CelebrationPassports.Web.ViewModels.Dashboard;

namespace CelebrationPassports.Web.Services;

// Only PassportProgress and Timeline are still mock (no Milestone/PassportStamp
// timeline endpoint wired up yet, and neither widget is currently rendered on the live
// Dashboard — see Index.cshtml). Everything else DashboardController overwrites with
// real data after calling this: Passports, UpcomingCelebrations, PendingInvitations,
// RecentChapters, RecentStories, MemoryHero, and every Summary count.
public class DashboardService : IDashboardService
{
    public DashboardViewModel GetDashboard()
    {
        return new DashboardViewModel
        {
            Summary = new DashboardSummary(),

            PassportProgress = new PassportProgress
            {
                Percentage = 68,
                CompletedItems = 34,
                TotalItems = 50
            },

            Timeline = new List<TimelineItem>
            {
                new TimelineItem
                {
                    Year = 2026,
                    Month = "DEC",
                    Title = "Wedding Anniversary",
                    Location = "Udaipur, Rajasthan",
                    ImageUrl="/images/udaipur.jpg",
                    StampColor="#7C3AED"
                },

                new TimelineItem
                {
                    Year = 2026,
                    Month = "JUL",
                    Title = "Son's Birthday",
                    Location = "Delhi",
                    ImageUrl="/images/udaipur.jpg",
                    StampColor="#34C759"
                },

                new TimelineItem
                {
                    Year = 2026,
                    Month = "MAY",
                    Title = "Goa Vacation",
                    Location = "Goa",
                    ImageUrl="/images/udaipur.jpg",
                    StampColor="#FF9500"
                },

                new TimelineItem
                {
                    Year = 2026,
                    Month = "FEB",
                    Title = "Valentine's Dinner",
                    Location = "Olive Bistro",
                    ImageUrl="/images/udaipur.jpg",
                    StampColor="#FF2D55"
                }
            },
        };
    }
}