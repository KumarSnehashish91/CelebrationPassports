using System.Security.Cryptography;
using CelebrationPassports.Application.Exceptions;
using CelebrationPassports.Application.Gifting.DTOs;
using CelebrationPassports.Application.Gifting.Interfaces;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Enums;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using FluentValidation;
using MediaEntity = CelebrationPassports.Persistence.Entities.Media;

namespace CelebrationPassports.Application.Gifting.Services;

public class PassportGiftService : IPassportGiftService
{
    // Mock payment — v1 has no real payment gateway. This is the single fixed price
    // shown/recorded; there's nowhere else in the app a real amount would come from.
    private const decimal GiftAmount = 999.00m;

    private const string CodeAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // no 0/O/1/I

    // Same seeded lookup AutoChapterClusteringService relies on for its own
    // auto-created chapters — a gifted chapter needs a Category just like any other.
    private const string DefaultCategoryName = "Everyday";

    private readonly IGenericRepository<PassportGift> _giftRepository;
    private readonly IGenericRepository<Passport> _passportRepository;
    private readonly IGenericRepository<PassportOwnershipHistory> _ownershipHistoryRepository;
    private readonly IGenericRepository<User> _userRepository;
    private readonly IGenericRepository<GiftDraft> _draftRepository;
    private readonly IGenericRepository<GiftPhoto> _photoRepository;
    private readonly IGenericRepository<GeneratedStory> _storyEntryRepository;
    private readonly IGenericRepository<Story> _storyRepository;
    private readonly IGenericRepository<Chapter> _chapterRepository;
    private readonly IGenericRepository<MediaEntity> _mediaRepository;
    private readonly IGenericRepository<Category> _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<PurchaseGiftRequest> _purchaseValidator;

    public PassportGiftService(
        IGenericRepository<PassportGift> giftRepository,
        IGenericRepository<Passport> passportRepository,
        IGenericRepository<PassportOwnershipHistory> ownershipHistoryRepository,
        IGenericRepository<User> userRepository,
        IGenericRepository<GiftDraft> draftRepository,
        IGenericRepository<GiftPhoto> photoRepository,
        IGenericRepository<GeneratedStory> storyEntryRepository,
        IGenericRepository<Story> storyRepository,
        IGenericRepository<Chapter> chapterRepository,
        IGenericRepository<MediaEntity> mediaRepository,
        IGenericRepository<Category> categoryRepository,
        IUnitOfWork unitOfWork,
        IValidator<PurchaseGiftRequest> purchaseValidator)
    {
        _giftRepository = giftRepository;
        _passportRepository = passportRepository;
        _ownershipHistoryRepository = ownershipHistoryRepository;
        _userRepository = userRepository;
        _draftRepository = draftRepository;
        _photoRepository = photoRepository;
        _storyEntryRepository = storyEntryRepository;
        _storyRepository = storyRepository;
        _chapterRepository = chapterRepository;
        _mediaRepository = mediaRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _purchaseValidator = purchaseValidator;
    }

    public async Task<PassportGiftDto> PurchaseAsync(Guid purchaserUserId, PurchaseGiftRequest request)
    {
        await _purchaseValidator.ValidateAndThrowAsync(request);

        var title = string.IsNullOrWhiteSpace(request.PassportTitle)
            ? $"{request.RecipientName.Trim()}'s Celebration Passport"
            : request.PassportTitle.Trim();

        var passport = new Passport
        {
            Id = Guid.NewGuid(),
            OwnerUserId = purchaserUserId,
            Title = title,
            Status = PassportStatus.GiftPending,
            CreatedOn = DateTime.UtcNow
        };

        await _passportRepository.AddAsync(passport);

        var gift = new PassportGift
        {
            Id = Guid.NewGuid(),
            PassportId = passport.Id,
            PurchasedByUserId = purchaserUserId,
            RecipientName = request.RecipientName.Trim(),
            RecipientEmail = string.IsNullOrWhiteSpace(request.RecipientEmail) ? null : request.RecipientEmail.Trim(),
            GiftMessage = string.IsNullOrWhiteSpace(request.GiftMessage) ? null : request.GiftMessage.Trim(),
            MessageType = string.IsNullOrWhiteSpace(request.GiftMessage) ? null : GiftMessageType.Written,
            Amount = GiftAmount,
            RedemptionCode = GenerateRedemptionCode(),
            Status = PassportGiftStatus.AwaitingClaim,
            PurchasedOn = DateTime.UtcNow
        };

        await _giftRepository.AddAsync(gift);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(gift);
    }

    public async Task<GiftClaimInfoDto?> GetClaimInfoAsync(string redemptionCode)
    {
        var gift = await FindByCodeAsync(redemptionCode);

        if (gift is null)
        {
            return null;
        }

        var passport = await _passportRepository.GetByIdAsync(gift.PassportId);
        var purchaser = await _userRepository.GetByIdAsync(gift.PurchasedByUserId);
        var draft = await _draftRepository.FirstOrDefaultAsync(d => d.PassportGiftId == gift.Id);

        string? coverPhotoUrl = null;
        string? pullQuoteText = null;

        if (draft is not null)
        {
            var firstPhoto = (await _photoRepository.FindAsync(p => p.GiftDraftId == draft.Id))
                .OrderBy(p => p.DisplayOrder)
                .FirstOrDefault();
            coverPhotoUrl = firstPhoto?.Url;

            var storyEntry = await _storyEntryRepository.FirstOrDefaultAsync(s => s.GiftDraftId == draft.Id);
            pullQuoteText = storyEntry?.PullQuoteText;
        }

        var isScheduledAndNotYetDue = gift.DeliveryMode == GiftDeliveryMode.Scheduled
            && gift.ScheduledDeliveryDate.HasValue
            && gift.ScheduledDeliveryDate.Value > DateTime.UtcNow;

        return new GiftClaimInfoDto
        {
            RecipientName = gift.RecipientName,
            GifterName = purchaser is null ? "Someone" : FormatName(purchaser),
            PassportTitle = passport?.Title ?? string.Empty,
            HasMessage = gift.MessageType.HasValue,
            MessageTypeLabel = gift.MessageType switch
            {
                GiftMessageType.Voice => "a voice message",
                GiftMessageType.Video => "a video message",
                GiftMessageType.Written => "a written note",
                _ => null
            },
            CoverPhotoUrl = coverPhotoUrl,
            PullQuoteText = pullQuoteText,
            AlreadyClaimed = gift.Status == PassportGiftStatus.Claimed,
            IsScheduledAndNotYetDue = isScheduledAndNotYetDue,
            ScheduledDeliveryDate = gift.ScheduledDeliveryDate
        };
    }

    public async Task<Guid?> ClaimAsync(Guid claimantUserId, string redemptionCode)
    {
        var gift = await FindByCodeAsync(redemptionCode);

        if (gift is null || gift.Status != PassportGiftStatus.AwaitingClaim)
        {
            return null;
        }

        if (gift.DeliveryMode == GiftDeliveryMode.Scheduled
            && gift.ScheduledDeliveryDate.HasValue
            && gift.ScheduledDeliveryDate.Value > DateTime.UtcNow)
        {
            return null;
        }

        var passport = await _passportRepository.GetByIdAsync(gift.PassportId)
            ?? throw new NotFoundException("Passport not found.");

        var previousOwnerId = passport.OwnerUserId;

        passport.OwnerUserId = claimantUserId;
        passport.Status = PassportStatus.Active;

        await _ownershipHistoryRepository.AddAsync(new PassportOwnershipHistory
        {
            Id = Guid.NewGuid(),
            PassportId = passport.Id,
            OldOwnerId = previousOwnerId,
            NewOwnerId = claimantUserId,
            TransferredOn = DateTime.UtcNow,
            TransferredBy = claimantUserId,
            Reason = "Gift redeemed"
        });

        gift.Status = PassportGiftStatus.Claimed;
        gift.ClaimedByUserId = claimantUserId;
        gift.ClaimedOn = DateTime.UtcNow;

        var draft = await _draftRepository.FirstOrDefaultAsync(d => d.PassportGiftId == gift.Id);

        if (draft is not null)
        {
            var storyEntry = await _storyEntryRepository.FirstOrDefaultAsync(s => s.GiftDraftId == draft.Id);

            if (storyEntry is not null)
            {
                await CreateGiftedChapterAsync(passport, draft, storyEntry);
            }
        }

        await _unitOfWork.SaveChangesAsync();

        return passport.Id;
    }

    public async Task<IReadOnlyList<PassportGiftDto>> GetMyPurchasesAsync(Guid purchaserUserId)
    {
        var gifts = await _giftRepository.FindAsync(g => g.PurchasedByUserId == purchaserUserId);

        return gifts.OrderByDescending(g => g.PurchasedOn).Select(MapToDto).ToList();
    }

    public async Task<GiftMessageDto?> GetMessageAsync(Guid userId, Guid passportId)
    {
        var gift = await _giftRepository.FirstOrDefaultAsync(g => g.PassportId == passportId);

        if (gift is null || !gift.MessageType.HasValue || gift.Status != PassportGiftStatus.Claimed
            || gift.ClaimedByUserId != userId)
        {
            return null;
        }

        var purchaser = await _userRepository.GetByIdAsync(gift.PurchasedByUserId);

        return new GiftMessageDto
        {
            Type = gift.MessageType.Value.ToString(),
            Text = gift.MessageType == GiftMessageType.Written ? gift.GiftMessage : null,
            MediaUrl = gift.MessageType is GiftMessageType.Voice or GiftMessageType.Video ? gift.MessageMediaUrl : null,
            GifterName = purchaser is null ? "Someone" : FormatName(purchaser)
        };
    }

    public async Task<ClaimedGiftSummaryDto?> GetClaimedGiftSummaryAsync(Guid userId, Guid passportId)
    {
        var gift = await _giftRepository.FirstOrDefaultAsync(g => g.PassportId == passportId);

        if (gift is null || gift.Status != PassportGiftStatus.Claimed || gift.ClaimedByUserId != userId)
        {
            return null;
        }

        var purchaser = await _userRepository.GetByIdAsync(gift.PurchasedByUserId);
        var passport = await _passportRepository.GetByIdAsync(passportId);

        var draft = await _draftRepository.FirstOrDefaultAsync(d => d.PassportGiftId == gift.Id);
        var photoCount = 0;

        if (draft is not null)
        {
            photoCount = (await _photoRepository.FindAsync(p => p.GiftDraftId == draft.Id)).Count;
        }

        var chapter = await _chapterRepository.FirstOrDefaultAsync(c => c.PassportId == passportId && c.Source == ChapterSource.Gifted);

        return new ClaimedGiftSummaryDto
        {
            GifterName = purchaser is null ? "Someone" : FormatName(purchaser),
            PassportTitle = passport?.Title ?? string.Empty,
            PhotoCount = photoCount,
            HasMessage = gift.MessageType.HasValue,
            ChapterId = chapter?.Id
        };
    }

    // Pre-populates the recipient's newly-claimed Passport with the gift-giver's
    // generated story as its first Chapter (gift-message-schedule-claim.md). The
    // narrative prose itself (opening/body/closing) has no home in Chapter/Story's
    // existing schema — same "narrative generated separately from structural content"
    // pattern already used by Recap — so it stays reachable via GiftDraft/GeneratedStory
    // rather than being duplicated into a new field here.
    private async Task CreateGiftedChapterAsync(Passport passport, GiftDraft draft, GeneratedStory storyEntry)
    {
        var category = (await _categoryRepository.FindAsync(c => c.Name == DefaultCategoryName)).FirstOrDefault()
            ?? throw new InvalidOperationException($"The '{DefaultCategoryName}' category is missing from the seeded Categories lookup.");

        var photos = (await _photoRepository.FindAsync(p => p.GiftDraftId == draft.Id))
            .OrderBy(p => p.DisplayOrder)
            .ToList();

        var eventDate = DateOnly.FromDateTime(DateTime.UtcNow);

        var story = new Story
        {
            Id = Guid.NewGuid(),
            PassportId = passport.Id,
            Title = storyEntry.Title,
            StartDate = eventDate,
            EndDate = eventDate,
            DisplayOrder = 0,
            CreatedAt = DateTime.UtcNow
        };

        await _storyRepository.AddAsync(story);

        var chapter = new Chapter
        {
            Id = Guid.NewGuid(),
            PassportId = passport.Id,
            StoryId = story.Id,
            CategoryId = category.Id,
            Title = storyEntry.Title,
            EventDate = eventDate,
            DisplayOrder = 0,
            Status = ChapterStatus.Confirmed,
            Source = ChapterSource.Gifted
        };

        await _chapterRepository.AddAsync(chapter);

        // Deliberately doesn't set Chapter.CoverMediaId here — same convention
        // AutoChapterClusteringService already follows for its own auto-created
        // chapters. Chapter and Media are both brand-new rows in this same
        // SaveChangesAsync; Chapter -> Media (CoverMediaId) and Media -> Chapter
        // (ChapterId) pointing at each unsaved other is a mutual FK EF Core can't
        // linearize into an insert order, and throws a circular-dependency error.
        foreach (var photo in photos)
        {
            var media = new MediaEntity
            {
                Id = Guid.NewGuid(),
                ChapterId = chapter.Id,
                UploadedBy = draft.SenderUserId,
                Url = photo.Url,
                Type = MediaType.Photo,
                UploadedOn = DateTime.UtcNow,
                PendingClustering = false
            };

            await _mediaRepository.AddAsync(media);
        }
    }

    private async Task<PassportGift?> FindByCodeAsync(string redemptionCode) =>
        await _giftRepository.FirstOrDefaultAsync(g => g.RedemptionCode == redemptionCode.Trim().ToUpperInvariant());

    private static string GenerateRedemptionCode() =>
        "GIFT-" + RandomNumberGenerator.GetString(CodeAlphabet, 10);

    private static string FormatName(User user)
    {
        var name = $"{user.FirstName} {user.LastName}".Trim();
        return string.IsNullOrWhiteSpace(name) ? "Someone" : name;
    }

    private static PassportGiftDto MapToDto(PassportGift gift) => new()
    {
        Id = gift.Id,
        PassportId = gift.PassportId,
        RecipientName = gift.RecipientName,
        RecipientEmail = gift.RecipientEmail,
        GiftMessage = gift.GiftMessage,
        Amount = gift.Amount,
        RedemptionCode = gift.RedemptionCode,
        Status = gift.Status.ToString(),
        PurchasedOn = gift.PurchasedOn,
        ClaimedOn = gift.ClaimedOn,
        DeliveryMode = gift.DeliveryMode.ToString(),
        ScheduledDeliveryDate = gift.ScheduledDeliveryDate
    };
}
