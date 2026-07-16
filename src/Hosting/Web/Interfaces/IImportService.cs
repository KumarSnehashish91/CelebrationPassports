using CelebrationPassports.Web.Models.Imports;

namespace CelebrationPassports.Web.Interfaces;

public interface IImportService
{
    Task<ImportJobViewModel?> StartGooglePhotosImportAsync(Guid passportId, IFormFile archive);

    Task<ImportJobViewModel?> GetStatusAsync(Guid jobId);
}
