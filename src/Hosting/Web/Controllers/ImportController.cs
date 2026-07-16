using CelebrationPassports.Web.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.Web.Controllers;

// "Import Existing Memories" (feature-backlog-v1.1.md, PRESERVE #1) — v1 supports a
// Google Photos Takeout export only. Scoped to the user's first Passport, same
// single-passport assumption IdeasController and others already make throughout Web.
[Authorize]
public class ImportController : Controller
{
    private const long MaxArchiveSizeBytes = 2L * 1024 * 1024 * 1024;

    private readonly IImportService _importService;
    private readonly IPassportService _passportService;

    public ImportController(IImportService importService, IPassportService passportService)
    {
        _importService = importService;
        _passportService = passportService;
    }

    public async Task<IActionResult> Index(string? error)
    {
        var passports = await _passportService.GetMineAsync();
        var passportId = passports.FirstOrDefault()?.Id;

        if (passportId is null)
        {
            return RedirectToAction("Create", "Passports");
        }

        ViewData["PassportId"] = passportId.Value;
        ViewData["Error"] = error;

        return View();
    }

    [HttpPost]
    [RequestSizeLimit(MaxArchiveSizeBytes)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxArchiveSizeBytes)]
    public async Task<IActionResult> StartGooglePhotosImport(Guid passportId, IFormFile archive)
    {
        if (archive.Length <= 0)
        {
            return RedirectToAction("Index", new { error = "Choose a .zip export to import." });
        }

        var job = await _importService.StartGooglePhotosImportAsync(passportId, archive);

        if (job is null)
        {
            return RedirectToAction("Index", new { error = "Couldn't start the import — please try again." });
        }

        return RedirectToAction("Status", new { jobId = job.Id });
    }

    public async Task<IActionResult> Status(Guid jobId)
    {
        var job = await _importService.GetStatusAsync(jobId);

        if (job is null)
        {
            return NotFound();
        }

        return View(job);
    }
}
