using CelebrationPassports.API.Extensions;
using CelebrationPassports.Application.TimeCapsule.DTOs;
using CelebrationPassports.Application.TimeCapsule.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.API.APIControllers.TimeCapsuleAPI;

[ApiController]
[Authorize]
public class TimeCapsuleMessagesController : ControllerBase
{
    private readonly ITimeCapsuleMessageService _messageService;

    public TimeCapsuleMessagesController(ITimeCapsuleMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpPost("api/passports/{passportId:guid}/time-capsule-messages")]
    public async Task<IActionResult> Create(Guid passportId, CreateTimeCapsuleMessageRequest request)
    {
        var result = await _messageService.CreateAsync(User.GetUserId(), passportId, request);
        return Ok(result);
    }

    [HttpGet("api/passports/{passportId:guid}/time-capsule-messages")]
    public async Task<IActionResult> GetByPassport(Guid passportId)
    {
        var result = await _messageService.GetByPassportAsync(User.GetUserId(), passportId);
        return Ok(result);
    }

    [HttpDelete("api/time-capsule-messages/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _messageService.DeleteAsync(User.GetUserId(), id);
        return NoContent();
    }
}
