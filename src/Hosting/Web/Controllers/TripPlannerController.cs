using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Events;
using CelebrationPassports.Web.Models.TripPlanner;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.Web.Controllers;

[Authorize]
public class TripPlannerController : Controller
{
    // Mirrors CelebrationPassports.Web.Models.Events.EventTypeOption's Vacation entry
    // (Value=4) — a "Trip" is a Vacation-type Event, reusing all of the Events feature's
    // existing status/schedule/cancel machinery instead of a separate domain concept.
    private const int VacationEventType = 4;

    private readonly ITripPlannerService _tripPlannerService;
    private readonly IEventService _eventService;
    private readonly IPassportService _passportService;
    private readonly ITripItineraryService _itineraryService;

    public TripPlannerController(
        ITripPlannerService tripPlannerService,
        IEventService eventService,
        IPassportService passportService,
        ITripItineraryService itineraryService)
    {
        _tripPlannerService = tripPlannerService;
        _eventService = eventService;
        _passportService = passportService;
        _itineraryService = itineraryService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new TripPlanViewModel { Days = 3 });
    }

    [HttpPost]
    public async Task<IActionResult> Index(TripPlanViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _tripPlannerService.GenerateAsync(model.Destination, model.Days, model.Notes);

        if (result is null)
        {
            ModelState.AddModelError("", "Could not generate a trip plan right now — the AI service may be unavailable. Please try again.");
            return View(model);
        }

        return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> SaveAsTrip(string destination, string? description, List<TripPlanDayViewModel>? itinerary)
    {
        if (string.IsNullOrWhiteSpace(destination))
        {
            return RedirectToAction("Index");
        }

        var passports = await _passportService.GetMineAsync();
        var passportId = passports.FirstOrDefault()?.Id;

        if (passportId is null)
        {
            return RedirectToAction("Create", "Passports");
        }

        var model = new EventWizardViewModel
        {
            PassportId = passportId.Value,
            Title = destination,
            EventType = VacationEventType,
            Description = description
        };

        var savedId = await _eventService.SaveAsync(model);

        if (savedId is null)
        {
            return RedirectToAction("Index");
        }

        // Persists the exact plan just generated, structured by day, so Trip Detail
        // shows it immediately — not just the flattened text folded into Notes above.
        if (itinerary is { Count: > 0 })
        {
            await _itineraryService.SaveItineraryAsync(savedId.Value, itinerary);
        }

        // Drops straight into Step 2 — the AI plan gives a destination and a
        // relative day-by-day itinerary but no real calendar dates yet.
        return RedirectToAction("DateTime", "Events", new { id = savedId });
    }
}
