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

    public DashboardController(
        IDashboardService dashboardService,
        IPassportService passportService,
        IEventService eventService,
        IInvitationService invitationService,
        IStoryService storyService,
        IDashboardStatsService statsService)
    {
        _dashboardService = dashboardService;
        _passportService = passportService;
        _eventService = eventService;
        _invitationService = invitationService;
        _storyService = storyService;
        _statsService = statsService;
    }

    public async Task<IActionResult> Index()
    {
        var passportsTask = _passportService.GetMineAsync();
        var invitationsTask = _invitationService.GetPendingAsync();

        await Task.WhenAll(passportsTask, invitationsTask);

        var passports = passportsTask.Result;
        var invitations = invitationsTask.Result;

        // Only push straight to onboarding if there's truly nothing to look at yet —
        // a user with a pending invitation still has something actionable to see here,
        // even with zero passports of their own.
        if (passports.Count == 0 && invitations.Count == 0)
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
        model.UpcomingCelebrations = upcomingTask.Result;
        model.RecentChapters = recentChaptersTask.Result;

        model.Summary.UpcomingCelebrations = upcomingTask.Result.Count;
        model.Summary.Invitations = invitations.Count;
        model.Summary.PassportStamps = stampCountTask.Result;
        model.Summary.Memories = statsTask.Result.MemoriesCount;
        model.Summary.Trips = statsTask.Result.TripsCount;
        model.Summary.Countries = statsTask.Result.CountriesCount;

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

        model.RecentStories = storiesByRecency
            .Where(s => s.Id != heroStory?.Id)
            .Take(4)
            .ToList();

        return View(model);
    }
}
