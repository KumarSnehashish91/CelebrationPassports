using CelebrationPassports.API.Extensions;
using CelebrationPassports.Application.Media.DTOs;
using CelebrationPassports.Application.Media.Interfaces;
using CelebrationPassports.Application.Stories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.API.APIControllers.MediaAPI;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MediaController : ControllerBase
{
    private readonly IMediaService _mediaService;
    private readonly ITripDetectionService _tripDetectionService;

    public MediaController(IMediaService mediaService, ITripDetectionService tripDetectionService)
    {
        _mediaService = mediaService;
        _tripDetectionService = tripDetectionService;
    }

    [HttpPost("chapters/{chapterId:guid}")]
    [RequestSizeLimit(25 * 1024 * 1024)]
    public async Task<IActionResult> Upload(Guid chapterId, IFormFile file)
    {
        var result = await UploadInternalAsync(chapterId, file);
        return Ok(result);
    }

    // Unattached upload — "upload first, sort later" (e.g. an Event's cover photo,
    // which has no Chapter/Story/Passport guard context of its own yet).
    [HttpPost]
    [RequestSizeLimit(25 * 1024 * 1024)]
    public async Task<IActionResult> UploadUnattached(IFormFile file)
    {
        var result = await UploadInternalAsync(null, file);
        return Ok(result);
    }

    private async Task<MediaDto> UploadInternalAsync(Guid? chapterId, IFormFile file)
    {
        await using var stream = file.OpenReadStream();

        return await _mediaService.UploadAsync(User.GetUserId(), chapterId, new FileUploadRequest
        {
            Content = stream,
            FileName = file.FileName,
            Length = file.Length
        });
    }

    [HttpGet("chapters/{chapterId:guid}")]
    public async Task<IActionResult> ListByChapter(Guid chapterId)
    {
        var result = await _mediaService.ListByChapterAsync(User.GetUserId(), chapterId);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediaService.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("batches/detect-trip")]
    public async Task<IActionResult> DetectTrip(DetectTripRequest request)
    {
        var chapterId = await _tripDetectionService.DetectAsync(User.GetUserId(), request.MediaIds);
        return Ok(new { detected = chapterId.HasValue, chapterId });
    }
}

public class DetectTripRequest
{
    public List<Guid> MediaIds { get; set; } = [];
}
