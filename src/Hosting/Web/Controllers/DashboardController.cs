using CelebrationPassports.Web.Interfaces;
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

    public DashboardController(
        IDashboardService dashboardService,
        IPassportService passportService,
        IEventService eventService,
        IInvitationService invitationService,
        IStoryService storyService)
    {
        _dashboardService = dashboardService;
        _passportService = passportService;
        _eventService = eventService;
        _invitationService = invitationService;
        _storyService = storyService;
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

        await Task.WhenAll(upcomingTask, stampCountTask, recentChaptersTask);

        var model = _dashboardService.GetDashboard();
        model.Passports = passports;
        model.PendingInvitations = invitations;
        model.UpcomingCelebrations = upcomingTask.Result;
        model.RecentChapters = recentChaptersTask.Result;

        model.Summary.UpcomingCelebrations = upcomingTask.Result.Count;
        model.Summary.Invitations = invitations.Count;
        model.Summary.PassportStamps = stampCountTask.Result;

        return View(model);
    }
}
