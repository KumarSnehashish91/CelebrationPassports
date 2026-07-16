using CelebrationPassports.Application.Gifting.DTOs;
using CelebrationPassports.Application.GiftStories.DTOs;
using CelebrationPassports.Application.Media.DTOs;
using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Application.GiftStories.Interfaces;

// Feature: Gift Photo Story & Print-Ready Preview (gift-story-print-preview.md) —
// extends Gift a Passport. A giver builds a GiftDraft through this four-step wizard
// (photos → insights → story → print), then FinalizeAsync hands off into the existing
// IPassportGiftService purchase flow.
public interface IGiftStoryService
{
    Task<GiftDraftDto> StartAsync(Guid senderUserId, PurchaseGiftRequest recipientInfo);

    Task<GiftDraftDto?> GetAsync(Guid senderUserId, Guid draftId);

    Task<GiftPhotoDto> AddPhotoAsync(Guid senderUserId, Guid draftId, FileUploadRequest file);

    Task RemovePhotoAsync(Guid senderUserId, Guid photoId);

    // Requires at least 4 photos (matches the ~4-5 photo minimum used elsewhere for
    // chapter formation) — throws ValidationException otherwise.
    Task<GiftDraftDto> ContinueToInsightsAsync(Guid senderUserId, Guid draftId);

    Task SetInsightsAsync(Guid senderUserId, Guid draftId, Dictionary<Guid, string?> insightsByPhotoId);

    // Fills AI insights for any photo left blank (vision model), then generates the
    // narrative. Safe to call once; RegenerateStoryAsync is the repeatable path.
    Task<GeneratedStoryDto> GenerateStoryAsync(Guid senderUserId, Guid draftId);

    // Never touches GiftPhoto.UserInsight — only the narrative changes.
    Task<GeneratedStoryDto> RegenerateStoryAsync(Guid senderUserId, Guid draftId);

    Task<GeneratedStoryDto?> GetStoryAsync(Guid senderUserId, Guid draftId);

    Task<PrintPreviewDto> GetPrintPreviewAsync(Guid senderUserId, Guid draftId, PrintFormat format);

    // Step 5 (gift-message-schedule-claim.md). mediaFile is used for Voice/Video;
    // writtenText for Written. Whichever isn't relevant to messageType is ignored.
    Task<GiftDraftDto> SetMessageAsync(
        Guid senderUserId, Guid draftId, GiftMessageType messageType, FileUploadRequest? mediaFile, string? writtenText);

    Task<GiftDraftDto> SetDeliveryScheduleAsync(Guid senderUserId, Guid draftId, GiftDeliveryMode mode, DateTime? scheduledDate);

    // "Add to Gift" — creates the Passport + PassportGift (via IPassportGiftService)
    // and marks this draft Sent.
    Task<PassportGiftDto> FinalizeAsync(Guid senderUserId, Guid draftId, PrintFormat format);
}
