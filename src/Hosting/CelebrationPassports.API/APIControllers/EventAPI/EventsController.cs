using CelebrationPassports.API.Extensions;
using CelebrationPassports.Application.Events.DTOs;
using CelebrationPassports.Application.Events.Interfaces;
using CelebrationPassports.Persistence.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.API.APIControllers.EventAPI;

[ApiController]
[Authorize]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    [HttpPost("api/passports/{passportId:guid}/events")]
    public async Task<IActionResult> Create(Guid passportId, CreateEventRequest request)
    {
        var result = await _eventService.CreateAsync(User.GetUserId(), passportId, request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("api/passports/{passportId:guid}/events")]
    public async Task<IActionResult> ListByPassport(Guid passportId, [FromQuery] EventStatus? status)
    {
        var result = await _eventService.ListByPassportAsync(User.GetUserId(), passportId, status);
        return Ok(result);
    }

    [HttpGet("api/events/mine/upcoming")]
    public async Task<IActionResult> UpcomingForMe([FromQuery] int take = 5)
    {
        var result = await _eventService.GetUpcomingForUserAsync(User.GetUserId(), take);
        return Ok(result);
    }

    [HttpGet("api/events/mine")]
    public async Task<IActionResult> AllForMe([FromQuery] EventStatus? status)
    {
        var result = await _eventService.GetAllForUserAsync(User.GetUserId(), status);
        return Ok(result);
    }

    [HttpGet("api/events/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _eventService.GetByIdAsync(User.GetUserId(), id);
        return Ok(result);
    }

    [HttpPut("api/events/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateEventRequest request)
    {
        var result = await _eventService.UpdateAsync(User.GetUserId(), id, request);
        return Ok(result);
    }

    [HttpPost("api/events/{eventId:guid}/calendar-events")]
    public async Task<IActionResult> AddCalendarEvent(Guid eventId, AddCalendarEventRequest request)
    {
        var result = await _eventService.AddCalendarEventAsync(User.GetUserId(), eventId, request);
        return Ok(result);
    }

    [HttpPost("api/events/{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var result = await _eventService.CancelAsync(User.GetUserId(), id);
        return Ok(result);
    }

    [HttpPut("api/events/{id:guid}/story")]
    public async Task<IActionResult> LinkStory(Guid id, LinkStoryRequest request)
    {
        var result = await _eventService.LinkStoryAsync(User.GetUserId(), id, request.StoryId);
        return Ok(result);
    }
}
