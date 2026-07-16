using CelebrationPassports.Application.Imports.DTOs;
using CelebrationPassports.Application.Media.DTOs;

namespace CelebrationPassports.Application.Imports.Interfaces;

public interface IImportService
{
    Task<ImportJobDto> StartGooglePhotosImportAsync(Guid userId, Guid passportId, FileUploadRequest archive);

    Task<ImportJobDto?> GetStatusAsync(Guid userId, Guid jobId);
}
