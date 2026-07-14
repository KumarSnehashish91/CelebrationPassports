using CelebrationPassports.API.Extensions;
using CelebrationPassports.Application.Stories.DTOs;
using CelebrationPassports.Application.Stories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.API.APIControllers.StoryAPI;

[ApiController]
[Authorize]
public class StoriesController : ControllerBase
{
    private readonly IStoryService _storyService;
    private readonly IChapterService _chapterService;

    public StoriesController(IStoryService storyService, IChapterService chapterService)
    {
        _storyService = storyService;
        _chapterService = chapterService;
    }

    [HttpPost("api/passports/{passportId:guid}/stories")]
    public async Task<IActionResult> Create(Guid passportId, CreateStoryRequest request)
    {
        var result = await _storyService.CreateAsync(User.GetUserId(), passportId, request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("api/passports/{passportId:guid}/stories")]
    public async Task<IActionResult> ListByPassport(Guid passportId)
    {
        var result = await _storyService.ListByPassportAsync(User.GetUserId(), passportId);
        return Ok(result);
    }

    [HttpGet("api/stories/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _storyService.GetByIdAsync(User.GetUserId(), id);
        return Ok(result);
    }

    [HttpPost("api/stories/{storyId:guid}/chapters")]
    public async Task<IActionResult> AddChapter(Guid storyId, CreateChapterRequest request)
    {
        var result = await _chapterService.CreateAsync(User.GetUserId(), storyId, request);
        return Ok(result);
    }
}
