using System.Security.Claims;
using CelebrationPassports.Web.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.Web.Controllers;

[Authorize]
public class TimeCapsuleController : Controller
{
    private readonly ITimeCapsuleService _timeCapsuleService;
    private readonly IPassportService _passportService;

    public TimeCapsuleController(ITimeCapsuleService timeCapsuleService, IPassportService passportService)
    {
        _timeCapsuleService = timeCapsuleService;
        _passportService = passportService;
    }

    public async Task<IActionResult> Index()
    {
        var passports = await _passportService.GetMineAsync();
        var passportId = passports.FirstOrDefault()?.Id;

        if (passportId is null)
        {
            return RedirectToAction("Create", "Passports");
        }

        ViewData["PassportId"] = passportId.Value;

        var messages = await _timeCapsuleService.GetByPassportAsync(passportId.Value);

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(currentUserId, out var userId))
        {
            foreach (var message in messages)
            {
                message.IsMine = message.AuthorUserId == userId;
            }
        }

        return View(messages);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid passportId, string title, string content, DateTime unlockDate)
    {
        if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(content))
        {
            var success = await _timeCapsuleService.CreateAsync(passportId, title, content, unlockDate);

            if (!success)
            {
                TempData["TimeCapsuleError"] = "Could not schedule that message — make sure the unlock date is in the future.";
            }
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _timeCapsuleService.DeleteAsync(id);
        return RedirectToAction("Index");
    }
}
