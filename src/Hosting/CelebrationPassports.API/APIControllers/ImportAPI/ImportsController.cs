using CelebrationPassports.API.Extensions;
using CelebrationPassports.Application.Imports.Interfaces;
using CelebrationPassports.Application.Media.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.API.APIControllers.ImportAPI;

[ApiController]
[Route("api/imports")]
[Authorize]
public class ImportsController : ControllerBase
{
    private const long MaxArchiveSizeBytes = 2L * 1024 * 1024 * 1024;

    private readonly IImportService _importService;

    public ImportsController(IImportService importService)
    {
        _importService = importService;
    }

    [HttpPost("passports/{passportId:guid}/google-photos")]
    [RequestSizeLimit(MaxArchiveSizeBytes)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxArchiveSizeBytes)]
    public async Task<IActionResult> StartGooglePhotosImport(Guid passportId, IFormFile archive)
    {
        await using var stream = archive.OpenReadStream();

        var result = await _importService.StartGooglePhotosImportAsync(User.GetUserId(), passportId, new FileUploadRequest
        {
            Content = stream,
            FileName = archive.FileName,
            Length = archive.Length
        });

        return Accepted(result);
    }

    [HttpGet("{jobId:guid}")]
    public async Task<IActionResult> GetStatus(Guid jobId)
    {
        var result = await _importService.GetStatusAsync(User.GetUserId(), jobId);
        return result is null ? NotFound() : Ok(result);
    }
}
