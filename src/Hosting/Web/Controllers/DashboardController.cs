using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;
    private readonly IPassportService _passportService;
    private readonly IEventService _eventService;
    private readonly IInvitationService _invitationService;
    private readonly IStoryService _storyService;
    private readonly IDashboardStatsService _statsService;
    private readonly IChapterSharingService _sharingService;

    public DashboardController(
        IDashboardService dashboardService,
        IPassportService passportService,
        IEventService eventService,
        IInvitationService invitationService,
        IStoryService storyService,
        IDashboardStatsService statsService,
        IChapterSharingService sharingService)
    {
        _dashboardService = dashboardService;
        _passportService = passportService;
        _eventService = eventService;
        _invitationService = invitationService;
        _storyService = storyService;
        _statsService = statsService;
        _sharingService = sharingService;
    }

    public async Task<IActionResult> Index()
    {
        var passportsTask = _passportService.GetMineAsync();
        var invitationsTask = _invitationService.GetPendingAsync();
        var chapterInvitationsTask = _sharingService.GetPendingForMeAsync();

        await Task.WhenAll(passportsTask, invitationsTask, chapterInvitationsTask);

        var passports = passportsTask.Result;
        var invitations = invitationsTask.Result;
        var chapterInvitations = chapterInvitationsTask.Result;

        // Only push straight to onboarding if there's truly nothing to look at yet —
        // a user with a pending invitation still has something actionable to see here,
        // even with zero passports of their own.
        if (passports.Count == 0 && invitations.Count == 0 && chapterInvitations.Count == 0)
        {
            return RedirectToAction("Create", "Passports");
        }

        var upcomingTask = _eventService.GetUpcomingAsync();
        var stampCountTask = _passportService.GetStampCountAsync();
        var recentChaptersTask = _storyService.GetRecentChaptersAsync();
        var statsTask = _statsService.GetSummaryAsync();
        var storiesTask = _storyService.GetMineAsync();

        await Task.WhenAll(upcomingTask, stampCountTask, recentChaptersTask, statsTask, storiesTask);

        var model = _dashboardService.GetDashboard();
        model.Passports = passports;
        model.PendingInvitations = invitations;
        model.PendingChapterInvitations = chapterInvitations;
        model.UpcomingCelebrations = upcomingTask.Result;
        model.RecentChapters = recentChaptersTask.Result;

        model.Summary.UpcomingCelebrations = upcomingTask.Result.Count;
        model.Summary.Invitations = invitations.Count;
        model.Summary.PassportStamps = stampCountTask.Result;
        model.Summary.Memories = statsTask.Result.MemoriesCount;
        model.Summary.Trips = statsTask.Result.TripsCount;
        model.Summary.Countries = statsTask.Result.CountriesCount;

        var hasPassport = passports.Count > 0;
        var hasInvitedSomeone = passports.Any(p => p.PeopleCount > 1);
        var hasFirstMemory = model.RecentChapters.Count > 0;

        model.OnboardingSteps =
        [
            new()
            {
                Number = 1,
                Title = "Create Your Passport",
                Description = "Your shared space is ready.",
                IsDone = hasPassport
            },
            new()
            {
                Number = 2,
                Title = "Invite Your Partner",
                Description = "Link accounts to share your story together.",
                IsDone = hasInvitedSomeone
            },
            new()
            {
                Number = 3,
                Title = "Add Your First Memory",
                Description = "Upload a few photos — we'll organize them for you.",
                IsDone = hasFirstMemory
            }
        ];

        // Matches the mockup's "New User — Empty Dashboard State": shown until the user
        // has their first real memory, regardless of how many events they've planned —
        // the stats/grid dashboard has nothing meaningful to show before that.
        model.ShowOnboarding = !hasFirstMemory;

        if (model.ShowOnboarding)
        {
            return View(model);
        }

        var storiesByRecency = storiesTask.Result
            .OrderByDescending(s => s.StartDate)
            .ToList();

        var heroStory = storiesByRecency.FirstOrDefault();

        if (heroStory is not null)
        {
            var photoCount = 0;
            var videoCount = 0;

            // Bounded to one story's chapters (not the whole library), same tradeoff as
            // the Trip Photos grid — cheap enough for a single hero card.
            foreach (var chapterSummary in heroStory.ChapterCount > 0
                ? (await _storyService.GetByIdAsync(heroStory.Id))?.Chapters ?? []
                : [])
            {
                var chapter = await _storyService.GetChapterByIdAsync(chapterSummary.Id);

                if (chapter is not null)
                {
                    photoCount += chapter.Media.Count(m => m.Type == 1);
                    videoCount += chapter.Media.Count(m => m.Type == 2);
                }
            }

            model.MemoryHero = new MemoryHighlight
            {
                Title = heroStory.Title,
                Location = heroStory.PlaceName ?? "",
                Date = heroStory.StartDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.Today,
                Photos = photoCount,
                Videos = videoCount,
                Chapters = heroStory.ChapterCount,
                ImageUrl = heroStory.CoverImageUrl ?? "/images/udaipur.jpg",
                StoryId = heroStory.Id
            };
        }

        var nonHeroStories = storiesByRecency.Where(s => s.Id != heroStory?.Id).ToList();

        model.RecentStories = nonHeroStories.Take(4).ToList();
        model.MoreStoriesCount = Math.Max(0, nonHeroStories.Count - model.RecentStories.Count);

        model.Timeline = storiesByRecency
            .Take(5)
            .Select((s, i) =>
            {
                var date = s.StartDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.Today;

                return new TimelineItem
                {
                    Year = date.Year,
                    Month = date.ToString("MMM").ToUpperInvariant(),
                    Title = s.Title,
                    Location = s.PlaceName ?? "",
                    ImageUrl = string.IsNullOrWhiteSpace(s.CoverImageUrl) ? "/images/udaipur.jpg" : s.CoverImageUrl,
                    StampColor = TimelineStampColors[i % TimelineStampColors.Length],
                    StoryId = s.Id
                };
            })
            .ToList();

        return View(model);
    }

    // On-brand rotation for the timeline's decorative stamp dots — variety without
    // reintroducing the old purple/rainbow palette.
    private static readonly string[] TimelineStampColors = ["#D4AF37", "#1E2937", "#7A9E7E", "#C97B4A"];
}
