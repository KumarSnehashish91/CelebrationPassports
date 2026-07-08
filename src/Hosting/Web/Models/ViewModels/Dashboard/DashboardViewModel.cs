using CelebrationPassports.Web.Models.Dashboard;

namespace CelebrationPassports.Web.ViewModels.Dashboard;

public class DashboardViewModel
{
    public DashboardSummary Summary { get; set; } = new();

    public List<UpcomingCelebration> UpcomingCelebrations { get; set; } = [];

    public List<RecentActivity> RecentActivities { get; set; } = [];

    public PassportProgress PassportProgress { get; set; } = new();

    public List<QuickAction> QuickActions { get; set; } = [];
    public MemoryHighlight MemoryHighlight { get; set; } = new();
    public MemoryHighlight MemoryHero { get; set; } = new();
    public List<TimelineItem> Timeline { get; set; } = new();
}
