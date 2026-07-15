using CelebrationPassports.Web.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.Web.Controllers;

[Authorize]
public class MemoryMapController : Controller
{
    private readonly IMemoryMapService _memoryMapService;
    private readonly IPassportService _passportService;

    public MemoryMapController(IMemoryMapService memoryMapService, IPassportService passportService)
    {
        _memoryMapService = memoryMapService;
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

        var pins = await _memoryMapService.GetByPassportAsync(passportId.Value);
        return View(pins);
    }
}
