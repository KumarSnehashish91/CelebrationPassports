using CelebrationPassports.Web.Models.Dashboard;
using CelebrationPassports.Web.Models.Invitations;
using CelebrationPassports.Web.Models.Passports;

namespace CelebrationPassports.Web.ViewModels.Dashboard;

public class DashboardViewModel
{
    public DashboardSummary Summary { get; set; } = new();

    // Real, API-backed data — everything else below is still DashboardService's mock
    // data, pending the endpoints for it (see DashboardService).
    public List<PassportListItemViewModel> Passports { get; set; } = [];

    public List<InvitationViewModel> PendingInvitations { get; set; } = [];

    public List<UpcomingCelebration> UpcomingCelebrations { get; set; } = [];

    public List<RecentActivity> RecentActivities { get; set; } = [];

    public PassportProgress PassportProgress { get; set; } = new();

    public List<QuickAction> QuickActions { get; set; } = [];
    public MemoryHighlight MemoryHighlight { get; set; } = new();
    public MemoryHighlight MemoryHero { get; set; } = new();
    public List<TimelineItem> Timeline { get; set; } = new();
}
