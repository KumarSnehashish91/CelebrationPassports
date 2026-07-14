using CelebrationPassports.Application.Exceptions;
using CelebrationPassports.Application.Media.DTOs;
using CelebrationPassports.Application.Media.Interfaces;
using CelebrationPassports.Application.Passports.Interfaces;
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
    private readonly IUnitOfWork _unitOfWork;

    public MediaService(
        IChapterRepository chapterRepository,
        IGenericRepository<MediaEntity> mediaRepository,
        IPassportAccessGuard accessGuard,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork)
    {
        _chapterRepository = chapterRepository;
        _mediaRepository = mediaRepository;
        _accessGuard = accessGuard;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
    }

    public async Task<MediaDto> UploadAsync(Guid userId, Guid chapterId, FileUploadRequest file)
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

        var chapter = await _chapterRepository.GetByIdWithMediaAsync(chapterId)
            ?? throw new NotFoundException("Chapter not found.");

        await _accessGuard.EnsureMemberAsync(userId, chapter.Story.PassportId);

        var url = await _fileStorageService.SaveAsync(file.Content, file.FileName);

        var media = new MediaEntity
        {
            Id = Guid.NewGuid(),
            ChapterId = chapterId,
            UploadedBy = userId,
            Url = url,
            Type = mediaType
        };

        await _mediaRepository.AddAsync(media);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(media);
    }

    public async Task<IReadOnlyList<MediaDto>> ListByChapterAsync(Guid userId, Guid chapterId)
    {
        var chapter = await _chapterRepository.GetByIdWithMediaAsync(chapterId)
            ?? throw new NotFoundException("Chapter not found.");

        await _accessGuard.EnsureMemberAsync(userId, chapter.Story.PassportId);

        return chapter.Media.Select(MapToDto).ToList();
    }

    private static MediaDto MapToDto(MediaEntity media) => new()
    {
        Id = media.Id,
        ChapterId = media.ChapterId,
        Url = media.Url,
        Type = media.Type,
        UploadedBy = media.UploadedBy
    };
}
