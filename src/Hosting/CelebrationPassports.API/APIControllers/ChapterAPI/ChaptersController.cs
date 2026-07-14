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
}
