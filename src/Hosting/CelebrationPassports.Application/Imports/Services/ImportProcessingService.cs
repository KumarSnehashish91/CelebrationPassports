using CelebrationPassports.Application.Imports.Interfaces;
using CelebrationPassports.Infrastructure.Imports.Interfaces;
using CelebrationPassports.Infrastructure.Storage.Interfaces;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Enums;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using MediaEntity = CelebrationPassports.Persistence.Entities.Media;

namespace CelebrationPassports.Application.Imports.Services;

// Bypasses IMediaService and builds Media rows directly — same precedent as
// GuestbookService.ApproveAsync, since this feature's file allowlist/size limit and
// (already-parsed) capture metadata don't match IMediaService.UploadAsync's shape (that
// method always re-derives capture data from EXIF, which is a guaranteed no-op here
// since ZipArchiveEntry streams are never seekable).
public class ImportProcessingService : IImportProcessingService
{
    private static readonly Dictionary<string, MediaType> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        [".jpg"] = MediaType.Photo,
        [".jpeg"] = MediaType.Photo,
        [".png"] = MediaType.Photo,
        [".heic"] = MediaType.Photo,
        [".webp"] = MediaType.Photo,
        [".gif"] = MediaType.Photo,
        [".mp4"] = MediaType.Video,
        [".mov"] = MediaType.Video,
        [".webm"] = MediaType.Video
    };

    // Same ceiling MediaService applies to every other upload path — a large video from
    // the export will be skipped and counted, not silently dropped.
    private const long MaxFileSizeBytes = 25 * 1024 * 1024;

    private readonly IGenericRepository<ImportJob> _jobRepository;
    private readonly IGenericRepository<MediaEntity> _mediaRepository;
    private readonly IGooglePhotosImportParser _parser;
    private readonly IFileStorageService _fileStorageService;
    private readonly IImportArchiveStorage _archiveStorage;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ImportProcessingService> _logger;

    public ImportProcessingService(
        IGenericRepository<ImportJob> jobRepository,
        IGenericRepository<MediaEntity> mediaRepository,
        IGooglePhotosImportParser parser,
        IFileStorageService fileStorageService,
        IImportArchiveStorage archiveStorage,
        IUnitOfWork unitOfWork,
        ILogger<ImportProcessingService> logger)
    {
        _jobRepository = jobRepository;
        _mediaRepository = mediaRepository;
        _parser = parser;
        _fileStorageService = fileStorageService;
        _archiveStorage = archiveStorage;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<bool> ProcessNextPendingAsync(CancellationToken cancellationToken)
    {
        var pending = await _jobRepository.FindAsync(j => j.Status == ImportJobStatus.Pending);
        var job = pending.OrderBy(j => j.CreatedOn).FirstOrDefault();

        if (job is null)
        {
            return false;
        }

        job.Status = ImportJobStatus.Processing;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        try
        {
            job.TotalItems = _parser.CountMediaEntries(job.ArchivePath);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _parser.ProcessArchiveAsync(job.ArchivePath, media => ProcessOneAsync(job, media, cancellationToken), cancellationToken);

            job.Status = ImportJobStatus.Completed;
        }
        catch (Exception ex)
        {
            job.Status = ImportJobStatus.Failed;
            job.ErrorMessage = "The import couldn't be processed — check the archive is a valid Google Photos export .zip.";
            _logger.LogError(ex, "Import job {JobId} failed.", job.Id);
        }
        finally
        {
            job.CompletedOn = DateTime.UtcNow;
            _archiveStorage.Delete(job.ArchivePath);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return true;
    }

    private async Task ProcessOneAsync(ImportJob job, ParsedImportMedia parsedMedia, CancellationToken cancellationToken)
    {
        try
        {
            var extension = Path.GetExtension(parsedMedia.FileName);

            if (!AllowedExtensions.TryGetValue(extension, out var mediaType) ||
                parsedMedia.Length <= 0 ||
                parsedMedia.Length > MaxFileSizeBytes)
            {
                job.SkippedItems++;
                return;
            }

            var url = await _fileStorageService.SaveAsync(parsedMedia.Content, parsedMedia.FileName, cancellationToken);

            var media = new MediaEntity
            {
                Id = Guid.NewGuid(),
                ChapterId = null,
                UploadedBy = job.CreatedByUserId,
                Url = url,
                Type = mediaType,
                Latitude = parsedMedia.Latitude,
                Longitude = parsedMedia.Longitude,
                CapturedAt = parsedMedia.CapturedAt,
                UploadedOn = DateTime.UtcNow,
                PendingClustering = true
            };

            await _mediaRepository.AddAsync(media);
            job.ProcessedItems++;
        }
        catch (Exception ex)
        {
            job.SkippedItems++;
            _logger.LogWarning(ex, "Skipped one file in import job {JobId}: {FileName}", job.Id, parsedMedia.FileName);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
