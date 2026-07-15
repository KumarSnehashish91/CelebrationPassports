using CelebrationPassports.Web.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.Web.Controllers;

[Authorize]
public class IdeasController : Controller
{
    private readonly ISomedayIdeaService _ideaService;
    private readonly IPassportService _passportService;

    public IdeasController(ISomedayIdeaService ideaService, IPassportService passportService)
    {
        _ideaService = ideaService;
        _passportService = passportService;
    }

    public async Task<IActionResult> Index()
    {
        var passports = await _passportService.GetMineAsync();
        var passportId = passports.FirstOrDefault()?.Id;

        if (passportId is null)
        {
            return RedirectToAction("Create", "Passports");
        }

        ViewData["PassportId"] = passportId.Value;

        var ideas = await _ideaService.GetByPassportAsync(passportId.Value);
        return View(ideas);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid passportId, string title, string? notes)
    {
        if (!string.IsNullOrWhiteSpace(title))
        {
            await _ideaService.CreateAsync(passportId, title, notes);
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _ideaService.DeleteAsync(id);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> ConvertToEvent(Guid id)
    {
        var eventId = await _ideaService.ConvertToEventAsync(id);

        if (eventId is null)
        {
            return RedirectToAction("Index");
        }

        return RedirectToAction("Create", "Events", new { id = eventId });
    }
}
