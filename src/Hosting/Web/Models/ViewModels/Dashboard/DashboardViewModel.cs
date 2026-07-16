using CelebrationPassports.Web.Models.Dashboard;
using CelebrationPassports.Web.Models.Invitations;
using CelebrationPassports.Web.Models.Passports;
using CelebrationPassports.Web.Models.Sharing;
using CelebrationPassports.Web.Models.Stories;

namespace CelebrationPassports.Web.ViewModels.Dashboard;

public class DashboardViewModel
{
    public DashboardSummary Summary { get; set; } = new();

    // Real, API-backed data — everything else below is still DashboardService's mock
    // data, pending the endpoints for it (see DashboardService).
    public List<PassportListItemViewModel> Passports { get; set; } = [];

    public List<InvitationViewModel> PendingInvitations { get; set; } = [];

    // Scoped Family Sharing (feature-backlog-v1.1.md, CELEBRATE #10) — chapter-scoped
    // invitations, distinct from the full-passport ones above.
    public List<ChapterInvitationViewModel> PendingChapterInvitations { get; set; } = [];

    public List<UpcomingCelebration> UpcomingCelebrations { get; set; } = [];

    // Real, API-backed — replaces the old mock "Recent Activity" widget.
    public List<ChapterDetailViewModel> RecentChapters { get; set; } = [];

    public PassportProgress PassportProgress { get; set; } = new();

    public List<QuickAction> QuickActions { get; set; } = [];

    // Null when the user has no stories yet — the hero card renders an honest empty
    // state instead of a blank/fake highlight in that case.
    public MemoryHighlight? MemoryHero { get; set; }

    // Real recent stories (excluding whichever one is the Hero), for the Memory
    // Highlights strip — replaces the old hardcoded picsum.photos placeholder cards.
    public List<StoryListItemViewModel> RecentStories { get; set; } = [];

    // How many more stories exist beyond what's shown in the Highlights strip — feeds
    // the "+N" tile. Zero means don't show that tile at all.
    public int MoreStoriesCount { get; set; }

    public List<TimelineItem> Timeline { get; set; } = new();

    // Matches the mockup's "New User — Empty Dashboard State" screen — shown instead of
    // the full stats/grid dashboard until the user has added their first memory.
    public bool ShowOnboarding { get; set; }

    public List<OnboardingStepViewModel> OnboardingSteps { get; set; } = [];
}
