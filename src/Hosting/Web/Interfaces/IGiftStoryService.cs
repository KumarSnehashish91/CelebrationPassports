using CelebrationPassports.Web.Models.Gifting;
using CelebrationPassports.Web.Models.GiftStories;

namespace CelebrationPassports.Web.Interfaces;

public interface IGiftStoryService
{
    Task<GiftDraftViewModel?> StartAsync(PurchaseGiftViewModel recipientInfo);

    Task<GiftDraftViewModel?> GetAsync(Guid draftId);

    Task<GiftPhotoViewModel?> AddPhotoAsync(Guid draftId, IFormFile file);

    Task<bool> RemovePhotoAsync(Guid photoId);

    Task<GiftDraftViewModel?> ContinueToInsightsAsync(Guid draftId);

    Task<bool> SetInsightsAsync(Guid draftId, Dictionary<Guid, string?> insights);

    Task<GeneratedStoryViewModel?> GenerateStoryAsync(Guid draftId);

    Task<GeneratedStoryViewModel?> RegenerateStoryAsync(Guid draftId);

    Task<GeneratedStoryViewModel?> GetStoryAsync(Guid draftId);

    Task<PrintPreviewViewModel?> GetPrintPreviewAsync(Guid draftId, string format);

    Task<GiftDraftViewModel?> SetMessageAsync(Guid draftId, string messageType, IFormFile? media, string? writtenText);

    Task<GiftDraftViewModel?> SetDeliveryScheduleAsync(Guid draftId, string mode, DateTime? scheduledDate);

    Task<PassportGiftViewModel?> FinalizeAsync(Guid draftId, string format);
}
