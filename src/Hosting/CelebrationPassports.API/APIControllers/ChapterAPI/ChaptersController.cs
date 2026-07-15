using CelebrationPassports.API.Extensions;
using CelebrationPassports.Application.Stories.DTOs;
using CelebrationPassports.Application.Stories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.API.APIControllers.ChapterAPI;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChaptersController : ControllerBase
{
    private readonly IChapterService _chapterService;

    public ChaptersController(IChapterService chapterService)
    {
        _chapterService = chapterService;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _chapterService.GetByIdAsync(User.GetUserId(), id);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateChapterRequest request)
    {
        var result = await _chapterService.UpdateAsync(User.GetUserId(), id, request);
        return Ok(result);
    }

    [HttpGet("mine/drafts")]
    public async Task<IActionResult> MyDrafts()
    {
        var result = await _chapterService.ListDraftsForUserAsync(User.GetUserId());
        return Ok(result);
    }

    [HttpGet("mine/recent")]
    public async Task<IActionResult> MyRecent([FromQuery] int take = 6)
    {
        var result = await _chapterService.ListRecentConfirmedForUserAsync(User.GetUserId(), take);
        return Ok(result);
    }

    [HttpPost("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id, ConfirmChapterRequest request)
    {
        var result = await _chapterService.ConfirmAsync(User.GetUserId(), id, request);
        return Ok(result);
    }

    [HttpPost("{id:guid}/discard")]
    public async Task<IActionResult> Discard(Guid id)
    {
        await _chapterService.DiscardAsync(User.GetUserId(), id);
        return NoContent();
    }

    // Matches the api/passports/{passportId}/<resource> convention used by
    // Events/Stories/SomedayIdeas, rather than api/chapters/... — Memory Map is
    // conceptually a passport-scoped view, even though the query lives on IChapterService.
    [HttpGet("/api/passports/{passportId:guid}/memory-map")]
    public async Task<IActionResult> GetMemoryMap(Guid passportId)
    {
        var result = await _chapterService.GetMemoryMapAsync(User.GetUserId(), passportId);
        return Ok(result);
    }
}
