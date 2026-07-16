using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Recap;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.Web.Controllers;

[Authorize]
public class RecapController : Controller
{
    private readonly IStoryService _storyService;
    private readonly IRecapService _recapService;

    public RecapController(IStoryService storyService, IRecapService recapService)
    {
        _storyService = storyService;
        _recapService = recapService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? year)
    {
        var stories = await _storyService.GetMineAsync();

        var moments = new List<RecapMomentViewModel>();

        // Bounded by the user's total story count (a household's worth) — same
        // fetch-all-chapters tradeoff already used for the Dashboard's Memory Hero and
        // Timeline widgets.
        foreach (var story in stories)
        {
            if (story.ChapterCount == 0)
            {
                continue;
            }

            var detail = await _storyService.GetByIdAsync(story.Id);

            if (detail is null)
            {
                continue;
            }

            moments.AddRange(detail.Chapters.Select(c => new RecapMomentViewModel
            {
                Title = c.Title,
                CategoryName = c.CategoryName,
                EventDate = c.EventDate,
                StoryTitle = story.Title
            }));
        }

        var availableYears = moments
            .Select(m => m.EventDate.Year)
            .Distinct()
            .OrderByDescending(y => y)
            .ToList();

        var selectedYear = year ?? availableYears.FirstOrDefault(DateTime.Today.Year);

        ViewData["AvailableYears"] = availableYears;
        ViewData["SelectedYear"] = selectedYear;

        var yearMoments = moments.Where(m => m.EventDate.Year == selectedYear).ToList();

        ViewData["MomentCount"] = yearMoments.Count;

        if (yearMoments.Count > 0)
        {
            ViewData["Recap"] = await _recapService.GenerateAsync(selectedYear, yearMoments);
        }

        return View();
    }
}
