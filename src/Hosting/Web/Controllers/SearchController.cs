using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.Web.Controllers;

// "Ask Your Passport" (feature-backlog-v1.1.md, PLAN #9) — v1.1-scoped version: plain
// case-insensitive substring search across the user's own Events, Stories, Passports,
// and People. No AI, no full-text index — this app's data volumes are a household's
// worth of content, so a few in-memory Contains() filters over data already fetched via
// existing services is plenty; the v1.2 natural-language stretch goal is explicitly a
// thin layer on top of this, not a replacement.
[Authorize]
public class SearchController : Controller
{
    private readonly IEventService _eventService;
    private readonly IStoryService _storyService;
    private readonly IPassportService _passportService;

    public SearchController(IEventService eventService, IStoryService storyService, IPassportService passportService)
    {
        _eventService = eventService;
        _storyService = storyService;
        _passportService = passportService;
    }

    public async Task<IActionResult> Index(string? q)
    {
        var query = q?.Trim() ?? string.Empty;
        var results = new List<SearchResultViewModel>();

        if (query.Length >= 2)
        {
            var eventsTask = _eventService.GetAllAsync();
            var storiesTask = _storyService.GetMineAsync();
            var passportsTask = _passportService.GetMineAsync();

            await Task.WhenAll(eventsTask, storiesTask, passportsTask);

            results.AddRange(eventsTask.Result
                .Where(e => Matches(e.Title, query) || Matches(e.PlaceName, query))
                .Select(e => new SearchResultViewModel
                {
                    Type = "Event",
                    Title = e.Title,
                    Subtitle = e.StartDate.ToString("MMM dd, yyyy") + (string.IsNullOrWhiteSpace(e.PlaceName) ? "" : $" · {e.PlaceName}"),
                    Url = Url.Action("Preview", "Events", new { id = e.Id }) ?? "#",
                    Icon = "bi-calendar-event"
                }));

            results.AddRange(storiesTask.Result
                .Where(s => Matches(s.Title, query) || Matches(s.PlaceName, query))
                .Select(s => new SearchResultViewModel
                {
                    Type = "Story",
                    Title = s.Title,
                    Subtitle = s.PlaceName ?? $"{s.ChapterCount} {(s.ChapterCount == 1 ? "chapter" : "chapters")}",
                    Url = Url.Action("Details", "Stories", new { id = s.Id }) ?? "#",
                    Icon = "bi-images"
                }));

            results.AddRange(passportsTask.Result
                .Where(p => Matches(p.Title, query))
                .Select(p => new SearchResultViewModel
                {
                    Type = "Passport",
                    Title = p.Title,
                    Subtitle = $"{p.PeopleCount} {(p.PeopleCount == 1 ? "person" : "people")}",
                    Url = Url.Action("Details", "Passports", new { id = p.Id }) ?? "#",
                    Icon = "bi-award"
                }));

            // Same per-passport fan-out PeopleController already uses — there's no
            // "all my people" endpoint, and a household has few enough passports for
            // this to be cheap.
            foreach (var passport in passportsTask.Result)
            {
                var detail = await _passportService.GetByIdAsync(passport.Id);

                if (detail is null)
                {
                    continue;
                }

                results.AddRange(detail.People
                    .Where(person => Matches(person.Name, query))
                    .Select(person => new SearchResultViewModel
                    {
                        Type = "Person",
                        Title = person.Name,
                        Subtitle = $"{person.Role} · {passport.Title}",
                        Url = Url.Action("Details", "Passports", new { id = passport.Id }) ?? "#",
                        Icon = "bi-person"
                    }));
            }
        }

        ViewData["Query"] = query;
        return View(results);
    }

    private static bool Matches(string? text, string query) =>
        !string.IsNullOrWhiteSpace(text) && text.Contains(query, StringComparison.OrdinalIgnoreCase);
}
