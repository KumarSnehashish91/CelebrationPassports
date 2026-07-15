using CelebrationPassports.API.Extensions;
using CelebrationPassports.Application.Someday.DTOs;
using CelebrationPassports.Application.Someday.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.API.APIControllers.SomedayAPI;

[ApiController]
[Authorize]
public class SomedayIdeasController : ControllerBase
{
    private readonly ISomedayIdeaService _ideaService;

    public SomedayIdeasController(ISomedayIdeaService ideaService)
    {
        _ideaService = ideaService;
    }

    [HttpPost("api/passports/{passportId:guid}/someday-ideas")]
    public async Task<IActionResult> Create(Guid passportId, CreateSomedayIdeaRequest request)
    {
        var result = await _ideaService.CreateAsync(User.GetUserId(), passportId, request);
        return Ok(result);
    }

    [HttpGet("api/passports/{passportId:guid}/someday-ideas")]
    public async Task<IActionResult> GetByPassport(Guid passportId)
    {
        var result = await _ideaService.GetByPassportAsync(User.GetUserId(), passportId);
        return Ok(result);
    }

    [HttpPut("api/someday-ideas/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateSomedayIdeaRequest request)
    {
        var result = await _ideaService.UpdateAsync(User.GetUserId(), id, request);
        return Ok(result);
    }

    [HttpDelete("api/someday-ideas/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _ideaService.DeleteAsync(User.GetUserId(), id);
        return NoContent();
    }

    [HttpPost("api/someday-ideas/{id:guid}/convert")]
    public async Task<IActionResult> ConvertToEvent(Guid id)
    {
        var result = await _ideaService.ConvertToEventAsync(User.GetUserId(), id);
        return Ok(result);
    }
}
