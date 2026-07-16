using CelebrationPassports.Web.Models.Gifting;

namespace CelebrationPassports.Web.Interfaces;

public interface IPassportGiftService
{
    Task<PassportGiftViewModel?> PurchaseAsync(PurchaseGiftViewModel model);

    Task<List<PassportGiftViewModel>> GetMyPurchasesAsync();

    Task<GiftClaimInfoViewModel?> GetClaimInfoAsync(string code);

    Task<Guid?> ClaimAsync(string code);

    Task<GiftMessageViewModel?> GetMessageAsync(Guid passportId);

    Task<ClaimedGiftSummaryViewModel?> GetClaimedGiftSummaryAsync(Guid passportId);
}
