using CelebrationPassports.API.Extensions;
using CelebrationPassports.Application.TripItinerary.DTOs;
using CelebrationPassports.Application.TripItinerary.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.API.APIControllers.TripItineraryAPI;

[ApiController]
[Authorize]
public class TripItineraryController : ControllerBase
{
    private readonly ITripItineraryService _itineraryService;

    public TripItineraryController(ITripItineraryService itineraryService)
    {
        _itineraryService = itineraryService;
    }

    [HttpPut("api/events/{eventId:guid}/itinerary")]
    public async Task<IActionResult> Save(Guid eventId, SaveItineraryRequest request)
    {
        var result = await _itineraryService.SaveItineraryAsync(User.GetUserId(), eventId, request);
        return Ok(result);
    }

    [HttpGet("api/events/{eventId:guid}/itinerary")]
    public async Task<IActionResult> Get(Guid eventId)
    {
        var result = await _itineraryService.GetByEventAsync(User.GetUserId(), eventId);
        return Ok(result);
    }
}
