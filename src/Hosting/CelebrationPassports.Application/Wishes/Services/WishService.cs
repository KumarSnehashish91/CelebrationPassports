using CelebrationPassports.Application.Exceptions;
using CelebrationPassports.Application.Passports.Interfaces;
using CelebrationPassports.Application.Wishes.DTOs;
using CelebrationPassports.Application.Wishes.Interfaces;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using FluentValidation;

namespace CelebrationPassports.Application.Wishes.Services;

public class WishService : IWishService
{
    private readonly IGenericRepository<Comment> _commentRepository;
    private readonly IGenericRepository<Chapter> _chapterRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IPassportAccessGuard _accessGuard;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateWishRequest> _createValidator;

    public WishService(
        IGenericRepository<Comment> commentRepository,
        IGenericRepository<Chapter> chapterRepository,
        IUserProfileRepository userProfileRepository,
        IPassportAccessGuard accessGuard,
        IUnitOfWork unitOfWork,
        IValidator<CreateWishRequest> createValidator)
    {
        _commentRepository = commentRepository;
        _chapterRepository = chapterRepository;
        _userProfileRepository = userProfileRepository;
        _accessGuard = accessGuard;
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
    }

    public async Task<WishDto> CreateAsync(Guid userId, Guid chapterId, CreateWishRequest request)
    {
        await _createValidator.ValidateAndThrowAsync(request);

        var chapter = await _chapterRepository.GetByIdAsync(chapterId)
            ?? throw new NotFoundException("Chapter not found.");

        await _accessGuard.EnsureMemberAsync(userId, chapter.PassportId);

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            ChapterId = chapterId,
            UserId = userId,
            Text = request.Text,
            CreatedAt = DateTime.UtcNow
        };

        await _commentRepository.AddAsync(comment);
        await _unitOfWork.SaveChangesAsync();

        var authorName = await ResolveAuthorNameAsync(userId);

        return MapToDto(comment, authorName);
    }

    public async Task<IReadOnlyList<WishDto>> GetByChapterAsync(Guid userId, Guid chapterId)
    {
        var chapter = await _chapterRepository.GetByIdAsync(chapterId)
            ?? throw new NotFoundException("Chapter not found.");

        await _accessGuard.EnsureMemberAsync(userId, chapter.PassportId);

        var comments = await _commentRepository.FindAsync(c => c.ChapterId == chapterId && !c.IsDeleted);

        var result = new List<WishDto>();

        foreach (var comment in comments.OrderByDescending(c => c.CreatedAt))
        {
            var authorName = await ResolveAuthorNameAsync(comment.UserId);
            result.Add(MapToDto(comment, authorName));
        }

        return result;
    }

    public async Task DeleteAsync(Guid userId, Guid wishId)
    {
        var comment = await _commentRepository.GetByIdAsync(wishId)
            ?? throw new NotFoundException("Wish not found.");

        if (comment.IsDeleted)
        {
            return;
        }

        if (comment.UserId != userId)
        {
            throw new ForbiddenAccessException("Only the author can delete this wish.");
        }

        comment.IsDeleted = true;
        comment.DeletedOn = DateTime.UtcNow;
        comment.DeletedBy = userId;

        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<string> ResolveAuthorNameAsync(Guid userId)
    {
        var profile = await _userProfileRepository.GetUserProfileByIdAsync(userId);

        if (profile is null)
        {
            return "Someone";
        }

        return string.IsNullOrWhiteSpace(profile.DisplayName)
            ? $"{profile.FirstName} {profile.LastName}".Trim()
            : profile.DisplayName;
    }

    private static WishDto MapToDto(Comment comment, string authorName) => new()
    {
        Id = comment.Id,
        ChapterId = comment.ChapterId!.Value,
        UserId = comment.UserId,
        AuthorName = authorName,
        Text = comment.Text,
        CreatedAt = comment.CreatedAt
    };
}
