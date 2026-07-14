using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Passports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.Web.Controllers;

[Authorize]
public class PassportsController : Controller
{
    private readonly IPassportService _passportService;
    private readonly IInvitationService _invitationService;

    public PassportsController(IPassportService passportService, IInvitationService invitationService)
    {
        _passportService = passportService;
        _invitationService = invitationService;
    }

    public async Task<IActionResult> Index()
    {
        var passports = await _passportService.GetMineAsync();
        return View(passports);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreatePassportViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePassportViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var success = await _passportService.CreateAsync(model);

        if (!success)
        {
            ModelState.AddModelError("", "Could not create the passport. Please try again.");
            return View(model);
        }

        return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    public async Task<IActionResult> Invite(Guid passportId, string email)
    {
        if (!string.IsNullOrWhiteSpace(email))
        {
            await _invitationService.InviteAsync(passportId, email);
        }

        return RedirectToAction("Index");
    }
}
