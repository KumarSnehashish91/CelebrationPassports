using CelebrationPassports.Application.Gifting.DTOs;

namespace CelebrationPassports.Application.Gifting.Interfaces;

public interface IPassportGiftService
{
    Task<PassportGiftDto> PurchaseAsync(Guid purchaserUserId, PurchaseGiftRequest request);

    // Public — no auth, only the redemption code guards it.
    Task<GiftClaimInfoDto?> GetClaimInfoAsync(string redemptionCode);

    // Returns the newly-owned PassportId, or null if the code is invalid or already claimed.
    Task<Guid?> ClaimAsync(Guid claimantUserId, string redemptionCode);

    Task<IReadOnlyList<PassportGiftDto>> GetMyPurchasesAsync(Guid purchaserUserId);

    // Post-claim reveal only — returns null if this passport has no gift message, or
    // the requesting user isn't the passport's current (claimed) owner.
    Task<GiftMessageDto?> GetMessageAsync(Guid userId, Guid passportId);

    // Dashboard first-run welcome banner — returns null if this passport wasn't a gift,
    // or userId isn't the one who claimed it.
    Task<ClaimedGiftSummaryDto?> GetClaimedGiftSummaryAsync(Guid userId, Guid passportId);
}
