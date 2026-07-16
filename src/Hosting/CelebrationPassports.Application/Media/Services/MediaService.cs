using CelebrationPassports.Application.Exceptions;
using CelebrationPassports.Application.Media.DTOs;
using CelebrationPassports.Application.Media.Interfaces;
using CelebrationPassports.Application.Passports.Interfaces;
using CelebrationPassports.Infrastructure.Photo.Interfaces;
using CelebrationPassports.Infrastructure.Storage.Interfaces;
using CelebrationPassports.Persistence.Enums;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using FluentValidation;
using MediaEntity = CelebrationPassports.Persistence.Entities.Media;

namespace CelebrationPassports.Application.Media.Services;

public class MediaService : IMediaService
{
    // No blob storage yet — local-disk placeholder (see LocalFileStorageService), so the
    // allowlist/size limit live here rather than in a shared upload-policy config.
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
        [".webm"] = MediaType.Video,
        [".mp3"] = MediaType.Audio,
        [".wav"] = MediaType.Audio,
        [".m4a"] = MediaType.Audio,
        [".pdf"] = MediaType.Document
    };

    private const long MaxFileSizeBytes = 25 * 1024 * 1024;

    private readonly IChapterRepository _chapterRepository;
    private readonly IGenericRepository<MediaEntity> _mediaRepository;
    private readonly IPassportAccessGuard _accessGuard;
    private readonly IFileStorageService _fileStorageService;
    private readonly IPhotoMetadataService _photoMetadataService;
    private readonly IUnitOfWork _unitOfWork;

    public MediaService(
        IChapterRepository chapterRepository,
        IGenericRepository<MediaEntity> mediaRepository,
        IPassportAccessGuard accessGuard,
        IFileStorageService fileStorageService,
        IPhotoMetadataService photoMetadataService,
        IUnitOfWork unitOfWork)
    {
        _chapterRepository = chapterRepository;
        _mediaRepository = mediaRepository;
        _accessGuard = accessGuard;
        _fileStorageService = fileStorageService;
        _photoMetadataService = photoMetadataService;
        _unitOfWork = unitOfWork;
    }

    public async Task<MediaDto> UploadAsync(Guid userId, Guid? chapterId, FileUploadRequest file, bool pendingClustering = false)
    {
        if (file.Length <= 0)
        {
            throw new ValidationException("The uploaded file is empty.");
        }

        if (file.Length > MaxFileSizeBytes)
        {
            throw new ValidationException("The uploaded file exceeds the 25 MB limit.");
        }

        var extension = Path.GetExtension(file.FileName);

        if (!AllowedExtensions.TryGetValue(extension, out var mediaType))
        {
            throw new ValidationException($"File type '{extension}' is not supported.");
        }

        if (chapterId.HasValue)
        {
            var chapterExists = await _chapterRepository.ExistsAsync(c => c.Id == chapterId.Value);

            if (!chapterExists)
            {
                throw new NotFoundException("Chapter not found.");
            }

            await _accessGuard.EnsureChapterAccessAsync(userId, chapterId.Value);
        }

        // GPS/capture-date only make sense for photos — extraction is a no-op (all
        // nulls) for anything else, including when the stream isn't seekable.
        var metadata = mediaType == MediaType.Photo
            ? _photoMetadataService.Extract(file.Content)
            : new PhotoMetadata();

        var url = await _fileStorageService.SaveAsync(file.Content, file.FileName);

        var media = new MediaEntity
        {
            Id = Guid.NewGuid(),
            ChapterId = chapterId,
            UploadedBy = userId,
            Url = url,
            Type = mediaType,
            Latitude = metadata.Latitude,
            Longitude = metadata.Longitude,
            CapturedAt = metadata.CapturedAt,
            UploadedOn = DateTime.UtcNow,
            PendingClustering = !chapterId.HasValue && pendingClustering
        };

        await _mediaRepository.AddAsync(media);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(media);
    }

    public async Task<IReadOnlyList<MediaDto>> ListByChapterAsync(Guid userId, Guid chapterId)
    {
        var chapter = await _chapterRepository.GetByIdWithMediaAsync(chapterId)
            ?? throw new NotFoundException("Chapter not found.");

        await _accessGuard.EnsureChapterAccessAsync(userId, chapterId);

        return chapter.Media.Select(MapToDto).ToList();
    }

    public async Task<MediaDto?> GetByIdAsync(Guid id)
    {
        var media = await _mediaRepository.GetByIdAsync(id);
        return media is null ? null : MapToDto(media);
    }

    private static MediaDto MapToDto(MediaEntity media) => new()
    {
        Id = media.Id,
        ChapterId = media.ChapterId,
        Url = media.Url,
        Type = media.Type,
        UploadedBy = media.UploadedBy,
        Latitude = media.Latitude,
        Longitude = media.Longitude,
        CapturedAt = media.CapturedAt
    };
}
