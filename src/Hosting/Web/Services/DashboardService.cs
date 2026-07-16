using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Dashboard;
using CelebrationPassports.Web.ViewModels.Dashboard;

namespace CelebrationPassports.Web.Services;

// Only PassportProgress is still mock (no Milestone/PassportStamp progress endpoint
// wired up yet, and that widget — _IndexProgress — isn't currently rendered on the live
// Dashboard; see Index.cshtml). Everything else DashboardController overwrites with real
// data after calling this: Passports, UpcomingCelebrations, PendingInvitations,
// RecentChapters, RecentStories, MemoryHero, Timeline, and every Summary count.
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
        };
    }
}