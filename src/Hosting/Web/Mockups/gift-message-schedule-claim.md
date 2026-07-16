# Feature Spec: Gift Personal Message, Scheduled Delivery &amp; Claim-to-Continue

**Status:** Ready for scoping / implementation
**Owner component:** `CelebrationPassports.Application` (`GiftPassports` slice, extended) + a new scheduled-delivery job + public (unauthenticated) claim endpoint
**Related docs:** `gift-story-print-preview.md` (Step 4 of the gift wizard this extends), `feature-backlog-v1.1.md` (Gift a Passport, item 14)
**Precedes:** Existing "Pay & Send Gift" checkout, already implemented

---

## 1. Summary

Adds a fifth step to the Gift Passport wizard (after Print Preview, before checkout) letting the giver attach a **voice, video, or written personal message**, and choose whether the gift **sends immediately or unlocks on a scheduled future date**. Also specs the **recipient-facing claim experience** — the public landing page a recipient sees when opening their gift link, and the **first-run onboarding** they see immediately after claiming, which is the single highest-leverage screen for turning a one-time gift into a retained user.

Three related but distinct pieces:
1. Personal message capture (Step 5 of the sender-side wizard)
2. Scheduled delivery (a delivery-time concern, not tightly coupled to the message feature — could ship independently)
3. Claim & Continue (recipient-facing, unauthenticated until claim, then first-run experience)

---

## 2. Scope

### In scope
- Voice message recording (browser mic capture, ≤60s, waveform UI) — reuses the existing voice-note infrastructure already used for Memory Creation (`MediaType = VoiceNote`)
- Video message recording/upload — new `MediaType` if one doesn't already cover video
- Written message fallback (the current implementation may already have a `PersonalMessage` field on the gift entity — confirm and reuse rather than duplicate)
- Scheduled delivery: "Send Right Away" vs. "Schedule for a Date" with a date picker
- Public, unauthenticated claim page at a tokenized URL (e.g., `/claim/{token}`)
- Claim action: creates the recipient's `User` account (or links to an existing one) and the actual `Passport`, pre-populated with the gifted `GeneratedStory` as its first chapter
- Post-claim first-run screen: shows the gifted story prominently, plus a 3-step onboarding checklist matching the existing empty-dashboard-state pattern already built for organic sign-ups

### Explicitly out of scope for this phase
- Editing or re-recording the message after the gift has been sent
- Recipient declining/returning a gift (no "decline" flow — out of scope until there's a real need for it)
- Multi-recipient / group gifting (flagged separately in the broader Gift a Passport feature list — not this spec)

---

## 3. Entity Changes Required

### Extend `GiftDraft` (from `gift-story-print-preview.md`)
| Field | Type | Notes |
|---|---|---|
| `MessageType` | `enum { Voice, Video, Written, None }` | |
| `MessageMediaUrl` | `string?` | Blob Storage reference for voice/video; null for Written/None |
| `WrittenMessage` | `string?` | May already exist as `PersonalMessage` on the current gift entity — reuse, don't duplicate |
| `DeliveryMode` | `enum { Immediate, Scheduled }` | |
| `ScheduledDeliveryDate` | `datetime?` | Null when `DeliveryMode = Immediate` |
| `ClaimToken` | `string` | Unique, unguessable token for the public claim URL — generate at gift creation, not at send time |
| `ClaimedAt` | `datetime?` | Null until the recipient claims |
| `ClaimedByUserId` | `Guid?` (FK → User) | Set on claim |

### New entity: `ClaimedPassportLink` (optional, only if you need to track claim-specific metadata separately from the passport itself)
Simplest approach: **don't create a new entity** — on claim, just create a normal `Passport` + `PassportMember` (recipient as Owner) and set `GiftDraft.ClaimedByUserId` / `ClaimedAt`. The gifted `GeneratedStory` becomes that passport's first `PassportMoment`/Chapter, tagged with `Origin = Gifted` (extends the existing `Origin` enum from the auto-chapter-detection spec — add a third value alongside `Manual` and `AiGenerated`).

---

## 4. Flow

### Step 5 (Sender) — Personal Message
- Message type selector (3 cards: Voice / Video / Written) — only one active at a time
- Voice: standard record → waveform preview → re-record option, same UX pattern as Memory Creation's voice note capture
- Video: record via browser camera API or upload an existing file
- Written: plain textarea, reuses existing `PersonalMessage` field if present
- Schedule selector: two radio options, date picker appears only when "Schedule for a Date" is selected
- Continue → proceeds to existing checkout (no change to payment flow itself)

### Scheduled Delivery (Backend)
- If `DeliveryMode = Immediate`: claim link becomes active and the notification/email to the recipient sends as soon as payment succeeds (existing checkout completion hook)
- If `DeliveryMode = Scheduled`: claim link and recipient notification stay dormant until `ScheduledDeliveryDate`. Needs a scheduled job (same debounced/delayed job infrastructure decision flagged in `auto-chapter-detection.md` — if Hangfire or similar gets adopted for that feature, reuse it here rather than building a second scheduling mechanism)
- **Important edge case:** the gift-giver has already paid at this point (checkout happens before the scheduled date arrives, not after). The scheduled date only gates *delivery/claim-availability*, not payment.

### Claim Page (Public, Unauthenticated)
- Route: `/claim/{ClaimToken}` — no login required to *view* this page
- Displays: sender's name, gift preview (story cover + title + one pull-quote from the story), a note about the personal message ("includes a voice message") without playing/revealing it yet
- "Claim Your Passport" button — clicking this is what requires authentication:
  - If the recipient already has an account, prompt sign-in, then claim
  - If not, prompt the existing Sign Up flow, then claim automatically on successful registration
- On claim: create `Passport`, create `PassportMember` (recipient, Owner role), copy the `GeneratedStory` in as the first Chapter, mark `GiftDraft.ClaimedAt`/`ClaimedByUserId`

### Post-Claim First-Run Screen
- This replaces the standard empty-dashboard-state (from the earlier Signup/Onboarding spec) **only for gift recipients** — organic sign-ups still see the plain empty state
- Shows the gifted story prominently at the top, personal message playable/viewable here (this is the reveal moment — don't show message content on the public claim page, only after claim + login)
- 3-step onboarding checklist: "Passport Claimed" (done) → "Add Your First Memory" → "Invite Someone Into It" — same visual pattern as the existing onboarding steps component, just re-sequenced since step 1 is already complete for a gift recipient

---

## 5. Suggested Application-Layer Contracts

```csharp
// Sender-side (Features/GiftPassports)
public record SetGiftMessageCommand(Guid GiftDraftId, MessageType Type, string? MediaUrl, string? WrittenText) : IRequest;
public record SetDeliveryScheduleCommand(Guid GiftDraftId, DeliveryMode Mode, DateTime? ScheduledDate) : IRequest;

// Recipient-side (new Features/GiftClaims slice — unauthenticated entry point)
public record GetClaimPreviewQuery(string ClaimToken) : IRequest<ClaimPreviewDto>; // public, no auth
public record ClaimGiftCommand(string ClaimToken, Guid ClaimingUserId) : IRequest<ClaimResultDto>; // requires auth
```

`GetClaimPreviewQuery` must be careful never to leak the actual message content (voice/video/written) — only metadata ("includes a voice message") — since this endpoint is unauthenticated by design. Confirm the message media URL itself isn't returned or is signed/expiring only after claim, not before.

---

## 6. Acceptance Criteria

- [ ] A gift with `DeliveryMode = Scheduled` does not send any recipient-facing notification or activate the claim link before `ScheduledDeliveryDate`, even though payment already completed.
- [ ] The public claim page never exposes the personal message content (audio/video file, or written text) prior to the recipient claiming and authenticating.
- [ ] Claiming a gift creates exactly one `Passport` with the gifted story as its first Chapter — claiming twice (e.g., double-click, or opening the link on two devices) does not create duplicate passports.
- [ ] A recipient without an existing account can go from claim link → sign up → claimed passport in one continuous flow, without losing the claim token along the way.
- [ ] The post-claim first-run screen is shown only once — returning to the dashboard afterward shows the normal dashboard, not the welcome screen again.
- [ ] `Origin = Gifted` is set on the first chapter created via claim, distinguishing it from `Manual` and `AiGenerated` chapters for future analytics/UI treatment.

## 7. Open Questions for the Implementer

1. Does a `PersonalMessage` field already exist on the current gift entity (visible in the shipped "Gift a Passport" screen)? If so, reuse it for `WrittenMessage` rather than creating a duplicate field.
2. What email/notification service is currently wired up for sending the recipient their claim link? This spec assumes one exists (referenced generically as "the existing checkout completion hook") — confirm before building the scheduled-send trigger.
3. Video message: confirm max file size/duration and whether client-side compression is needed before upload, given mobile network conditions.
4. Should there be a reminder notification if a gift is claimed but the recipient never adds anything beyond the gifted story (e.g., a nudge at day 7)? Not in scope here, but worth flagging as a natural follow-up to the onboarding checklist.
