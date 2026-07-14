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
}
