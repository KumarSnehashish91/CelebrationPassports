using CelebrationPassports.Application.Imports.DTOs;
using CelebrationPassports.Application.Imports.Interfaces;
using CelebrationPassports.Application.Media.DTOs;
using CelebrationPassports.Application.Passports.Interfaces;
using CelebrationPassports.Infrastructure.Storage.Interfaces;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Enums;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using FluentValidation;

namespace CelebrationPassports.Application.Imports.Services;

public class ImportService : IImportService
{
    // A household's worth of a Google Photos export can easily run into the hundreds of
    // MB — well past MediaService's 25MB single-file cap — but still needs a ceiling
    // against abuse.
    private const long MaxArchiveSizeBytes = 2L * 1024 * 1024 * 1024;

    private readonly IGenericRepository<ImportJob> _jobRepository;
    private readonly IPassportAccessGuard _accessGuard;
    private readonly IImportArchiveStorage _archiveStorage;
    private readonly IUnitOfWork _unitOfWork;

    public ImportService(
        IGenericRepository<ImportJob> jobRepository,
        IPassportAccessGuard accessGuard,
        IImportArchiveStorage archiveStorage,
        IUnitOfWork unitOfWork)
    {
        _jobRepository = jobRepository;
        _accessGuard = accessGuard;
        _archiveStorage = archiveStorage;
        _unitOfWork = unitOfWork;
    }

    public async Task<ImportJobDto> StartGooglePhotosImportAsync(Guid userId, Guid passportId, FileUploadRequest archive)
    {
        if (archive.Length <= 0)
        {
            throw new ValidationException("The uploaded archive is empty.");
        }

        if (archive.Length > MaxArchiveSizeBytes)
        {
            throw new ValidationException("The uploaded archive exceeds the 2 GB limit.");
        }

        if (!string.Equals(Path.GetExtension(archive.FileName), ".zip", StringComparison.OrdinalIgnoreCase))
        {
            throw new ValidationException("Only a .zip export is supported.");
        }

        await _accessGuard.EnsureMemberAsync(userId, passportId);

        var job = new ImportJob
        {
            Id = Guid.NewGuid(),
            PassportId = passportId,
            CreatedByUserId = userId,
            SourceType = ImportSourceType.GooglePhotos,
            Status = ImportJobStatus.Pending,
            CreatedOn = DateTime.UtcNow
        };

        job.ArchivePath = await _archiveStorage.SaveAsync(archive.Content, job.Id);

        await _jobRepository.AddAsync(job);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(job);
    }

    public async Task<ImportJobDto?> GetStatusAsync(Guid userId, Guid jobId)
    {
        var job = await _jobRepository.GetByIdAsync(jobId);

        if (job is null)
        {
            return null;
        }

        await _accessGuard.EnsureMemberAsync(userId, job.PassportId);

        return MapToDto(job);
    }

    private static ImportJobDto MapToDto(ImportJob job) => new()
    {
        Id = job.Id,
        PassportId = job.PassportId,
        Status = job.Status.ToString(),
        TotalItems = job.TotalItems,
        ProcessedItems = job.ProcessedItems,
        SkippedItems = job.SkippedItems,
        ErrorMessage = job.ErrorMessage,
        CreatedOn = job.CreatedOn,
        CompletedOn = job.CompletedOn
    };
}
