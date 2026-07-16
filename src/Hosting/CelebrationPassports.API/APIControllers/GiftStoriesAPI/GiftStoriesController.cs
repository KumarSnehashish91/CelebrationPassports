using CelebrationPassports.API.Extensions;
using CelebrationPassports.Application.Gifting.DTOs;
using CelebrationPassports.Application.GiftStories.Interfaces;
using CelebrationPassports.Application.Media.DTOs;
using CelebrationPassports.Persistence.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.API.APIControllers.GiftStoriesAPI;

[ApiController]
[Route("api/gift-stories")]
[Authorize]
public class GiftStoriesController : ControllerBase
{
    private readonly IGiftStoryService _giftStoryService;

    public GiftStoriesController(IGiftStoryService giftStoryService)
    {
        _giftStoryService = giftStoryService;
    }

    [HttpPost]
    public async Task<IActionResult> Start(PurchaseGiftRequest request)
    {
        var result = await _giftStoryService.StartAsync(User.GetUserId(), request);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _giftStoryService.GetAsync(User.GetUserId(), id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{id:guid}/photos")]
    [RequestSizeLimit(25 * 1024 * 1024)]
    public async Task<IActionResult> AddPhoto(Guid id, IFormFile file)
    {
        await using var stream = file.OpenReadStream();

        var result = await _giftStoryService.AddPhotoAsync(User.GetUserId(), id, new FileUploadRequest
        {
            Content = stream,
            FileName = file.FileName,
            Length = file.Length
        });

        return Ok(result);
    }

    [HttpDelete("photos/{photoId:guid}")]
    public async Task<IActionResult> RemovePhoto(Guid photoId)
    {
        await _giftStoryService.RemovePhotoAsync(User.GetUserId(), photoId);
        return NoContent();
    }

    [HttpPost("{id:guid}/continue-to-insights")]
    public async Task<IActionResult> ContinueToInsights(Guid id)
    {
        var result = await _giftStoryService.ContinueToInsightsAsync(User.GetUserId(), id);
        return Ok(result);
    }

    [HttpPost("{id:guid}/insights")]
    public async Task<IActionResult> SetInsights(Guid id, [FromBody] Dictionary<Guid, string?> insights)
    {
        await _giftStoryService.SetInsightsAsync(User.GetUserId(), id, insights);
        return NoContent();
    }

    [HttpPost("{id:guid}/generate")]
    public async Task<IActionResult> Generate(Guid id)
    {
        var result = await _giftStoryService.GenerateStoryAsync(User.GetUserId(), id);
        return Ok(result);
    }

    [HttpPost("{id:guid}/regenerate")]
    public async Task<IActionResult> Regenerate(Guid id)
    {
        var result = await _giftStoryService.RegenerateStoryAsync(User.GetUserId(), id);
        return Ok(result);
    }

    [HttpGet("{id:guid}/story")]
    public async Task<IActionResult> GetStory(Guid id)
    {
        var result = await _giftStoryService.GetStoryAsync(User.GetUserId(), id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("{id:guid}/print-preview")]
    public async Task<IActionResult> GetPrintPreview(Guid id, [FromQuery] PrintFormat format = PrintFormat.LifeBook)
    {
        var result = await _giftStoryService.GetPrintPreviewAsync(User.GetUserId(), id, format);
        return Ok(result);
    }

    [HttpPost("{id:guid}/message")]
    [RequestSizeLimit(25 * 1024 * 1024)]
    public async Task<IActionResult> SetMessage(Guid id, [FromForm] SetGiftMessageFormModel form)
    {
        if (!Enum.TryParse<GiftMessageType>(form.MessageType, ignoreCase: true, out var messageType))
        {
            return BadRequest("Unrecognized message type.");
        }

        FileUploadRequest? mediaFile = null;
        Stream? stream = null;

        try
        {
            if (form.Media is { Length: > 0 })
            {
                stream = form.Media.OpenReadStream();
                mediaFile = new FileUploadRequest
                {
                    Content = stream,
                    FileName = form.Media.FileName,
                    Length = form.Media.Length
                };
            }

            var result = await _giftStoryService.SetMessageAsync(User.GetUserId(), id, messageType, mediaFile, form.WrittenText);
            return Ok(result);
        }
        finally
        {
            if (stream is not null)
            {
                await stream.DisposeAsync();
            }
        }
    }

    [HttpPost("{id:guid}/schedule")]
    public async Task<IActionResult> SetDeliverySchedule(Guid id, [FromBody] SetDeliveryScheduleRequest request)
    {
        if (!Enum.TryParse<GiftDeliveryMode>(request.Mode, ignoreCase: true, out var mode))
        {
            return BadRequest("Unrecognized delivery mode.");
        }

        var result = await _giftStoryService.SetDeliveryScheduleAsync(User.GetUserId(), id, mode, request.ScheduledDate);
        return Ok(result);
    }

    [HttpPost("{id:guid}/finalize")]
    public async Task<IActionResult> Finalize(Guid id, [FromBody] FinalizeGiftStoryRequest request)
    {
        if (!Enum.TryParse<PrintFormat>(request.Format, ignoreCase: true, out var format))
        {
            return BadRequest("Unrecognized print format.");
        }

        var result = await _giftStoryService.FinalizeAsync(User.GetUserId(), id, format);
        return Ok(result);
    }
}

public class FinalizeGiftStoryRequest
{
    // String, not the enum — System.Text.Json's default body-model-binding requires
    // the numeric value for enums, unlike query/route binding (EnumModelBinder), which
    // accepts names natively. Every other cross-boundary enum in this app uses a
    // string for the same reason.
    public string Format { get; set; } = string.Empty;
}

public class SetGiftMessageFormModel
{
    public string MessageType { get; set; } = string.Empty;

    public IFormFile? Media { get; set; }

    public string? WrittenText { get; set; }
}

public class SetDeliveryScheduleRequest
{
    public string Mode { get; set; } = string.Empty;

    public DateTime? ScheduledDate { get; set; }
}
