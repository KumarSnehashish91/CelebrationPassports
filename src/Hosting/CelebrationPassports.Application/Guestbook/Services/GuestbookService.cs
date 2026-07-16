using CelebrationPassports.Application.Exceptions;
using CelebrationPassports.Application.Guestbook.DTOs;
using CelebrationPassports.Application.Guestbook.Interfaces;
using CelebrationPassports.Application.Media.DTOs;
using CelebrationPassports.Application.Passports.Interfaces;
using CelebrationPassports.Infrastructure.Storage.Interfaces;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Enums;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using FluentValidation;
using MediaEntity = CelebrationPassports.Persistence.Entities.Media;

namespace CelebrationPassports.Application.Guestbook.Services;

// Feature: Digital Guestbook Mode (feature-backlog-v1.1.md, CELEBRATE #11). A guest
// with the unguessable per-chapter link can leave a name + note + one photo without an
// account. Nothing they submit is visible anywhere — not the chapter, not search, not
// Memory Map — until a real passport member approves it. The anonymous submit path is
// intentionally isolated from MediaService's authenticated upload pipeline (own, much
// stricter file allowlist/size cap, no chapter access-guard since there's no user to
// check) so a bug or abuse here can't touch anything the guest hasn't been explicitly
// invited to see.
public class GuestbookService : IGuestbookService
{
    // Deliberately tighter than MediaService's 25MB/photo+video+audio+document allowlist
    // — anonymous submitters only ever need to attach one phone photo.
    private static readonly HashSet<string> AllowedPhotoExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp", ".heic" };

    private const long MaxPhotoSizeBytes = 5 * 1024 * 1024;

    private readonly IGenericRepository<Chapter> _chapterRepository;
    private readonly IGenericRepository<GuestbookSubmission> _submissionRepository;
    private readonly IGenericRepository<MediaEntity> _mediaRepository;
    private readonly IGuestbookTokenService _tokenService;
    private readonly IPassportAccessGuard _accessGuard;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<SubmitGuestbookEntryRequest> _submitValidator;

    public GuestbookService(
        IGenericRepository<Chapter> chapterRepository,
        IGenericRepository<GuestbookSubmission> submissionRepository,
        IGenericRepository<MediaEntity> mediaRepository,
        IGuestbookTokenService tokenService,
        IPassportAccessGuard accessGuard,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork,
        IValidator<SubmitGuestbookEntryRequest> submitValidator)
    {
        _chapterRepository = chapterRepository;
        _submissionRepository = submissionRepository;
        _mediaRepository = mediaRepository;
        _tokenService = tokenService;
        _accessGuard = accessGuard;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
        _submitValidator = submitValidator;
    }

    public async Task<string> GetShareTokenAsync(Guid userId, Guid chapterId)
    {
        var chapter = await _chapterRepository.GetByIdAsync(chapterId)
            ?? throw new NotFoundException("Chapter not found.");

        await _accessGuard.EnsureMemberAsync(userId, chapter.PassportId);

        return _tokenService.GenerateToken(chapterId);
    }

    public async Task<GuestbookChapterInfoDto> GetPublicInfoAsync(Guid chapterId, string token)
    {
        if (!_tokenService.ValidateToken(chapterId, token))
        {
            throw new ForbiddenAccessException("This guestbook link is invalid.");
        }

        var chapter = await _chapterRepository.GetByIdAsync(chapterId)
            ?? throw new NotFoundException("Chapter not found.");

        return new GuestbookChapterInfoDto
        {
            ChapterTitle = chapter.Title,
            EventDate = chapter.EventDate
        };
    }

    public async Task SubmitAsync(Guid chapterId, string token, SubmitGuestbookEntryRequest request, FileUploadRequest? photo)
    {
        if (!_tokenService.ValidateToken(chapterId, token))
        {
            throw new ForbiddenAccessException("This guestbook link is invalid.");
        }

        await _submitValidator.ValidateAndThrowAsync(request);

        var chapter = await _chapterRepository.GetByIdAsync(chapterId)
            ?? throw new NotFoundException("Chapter not found.");

        string? photoUrl = null;

        if (photo is not null)
        {
            if (photo.Length <= 0)
            {
                throw new ValidationException("The uploaded photo is empty.");
            }

            if (photo.Length > MaxPhotoSizeBytes)
            {
                throw new ValidationException("The photo exceeds the 5 MB limit.");
            }

            var extension = Path.GetExtension(photo.FileName);

            if (!AllowedPhotoExtensions.Contains(extension))
            {
                throw new ValidationException($"Photo type '{extension}' is not supported — please use JPG, PNG, WEBP, or HEIC.");
            }

            photoUrl = await _fileStorageService.SaveAsync(photo.Content, photo.FileName);
        }

        var submission = new GuestbookSubmission
        {
            Id = Guid.NewGuid(),
            ChapterId = chapterId,
            GuestName = request.GuestName.Trim(),
            Message = string.IsNullOrWhiteSpace(request.Message) ? null : request.Message.Trim(),
            PhotoUrl = photoUrl,
            Status = GuestbookSubmissionStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _submissionRepository.AddAsync(submission);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<GuestbookSubmissionDto>> GetPendingAsync(Guid userId, Guid chapterId)
    {
        var chapter = await _chapterRepository.GetByIdAsync(chapterId)
            ?? throw new NotFoundException("Chapter not found.");

        await _accessGuard.EnsureMemberAsync(userId, chapter.PassportId);

        var submissions = await _submissionRepository.FindAsync(
            s => s.ChapterId == chapterId && s.Status == GuestbookSubmissionStatus.Pending);

        return submissions.OrderBy(s => s.CreatedAt).Select(MapToDto).ToList();
    }

    public async Task ApproveAsync(Guid userId, Guid submissionId)
    {
        var submission = await _submissionRepository.GetByIdAsync(submissionId)
            ?? throw new NotFoundException("Submission not found.");

        if (submission.Status != GuestbookSubmissionStatus.Pending)
        {
            return;
        }

        var chapter = await _chapterRepository.GetByIdAsync(submission.ChapterId)
            ?? throw new NotFoundException("Chapter not found.");

        await _accessGuard.EnsureMemberAsync(userId, chapter.PassportId);

        if (!string.IsNullOrWhiteSpace(submission.PhotoUrl))
        {
            var media = new MediaEntity
            {
                Id = Guid.NewGuid(),
                ChapterId = submission.ChapterId,
                UploadedBy = userId,
                Url = submission.PhotoUrl,
                Type = MediaType.Photo,
                UploadedOn = DateTime.UtcNow,
                PendingClustering = false
            };

            await _mediaRepository.AddAsync(media);
            submission.ApprovedMediaId = media.Id;
        }

        submission.Status = GuestbookSubmissionStatus.Approved;
        submission.ReviewedByUserId = userId;
        submission.ReviewedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RejectAsync(Guid userId, Guid submissionId)
    {
        var submission = await _submissionRepository.GetByIdAsync(submissionId)
            ?? throw new NotFoundException("Submission not found.");

        if (submission.Status != GuestbookSubmissionStatus.Pending)
        {
            return;
        }

        var chapter = await _chapterRepository.GetByIdAsync(submission.ChapterId)
            ?? throw new NotFoundException("Chapter not found.");

        await _accessGuard.EnsureMemberAsync(userId, chapter.PassportId);

        submission.Status = GuestbookSubmissionStatus.Rejected;
        submission.ReviewedByUserId = userId;
        submission.ReviewedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();
    }

    private static GuestbookSubmissionDto MapToDto(GuestbookSubmission s) => new()
    {
        Id = s.Id,
        ChapterId = s.ChapterId,
        GuestName = s.GuestName,
        Message = s.Message,
        PhotoUrl = s.PhotoUrl,
        Status = s.Status,
        CreatedAt = s.CreatedAt
    };
}
