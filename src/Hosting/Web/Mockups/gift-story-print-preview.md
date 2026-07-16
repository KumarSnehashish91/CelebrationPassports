# Feature Spec: Gift Passport — Photo Story & Print-Ready Preview

**Status:** Ready for scoping / implementation
**Owner component:** `CelebrationPassports.Application` (new `GiftPassports` feature slice) + `AI Services` (Ollama) + a new print-rendering capability
**Related docs:** `Celebration_Passport_System_Design_v1.docx`, `Celebration_Passport_Entity_Reference_ERD_v0.1.docx`, `feature-backlog-v1.1.md` (Gift a Passport, item 14)

---

## 1. Summary

Extends the existing "Gift a Passport" flow. Today, gifting creates an empty passport for the recipient to fill in themselves. This feature lets the **gift-giver** upload photos of the recipient, optionally add a short note ("insight") per photo, and have the system generate a complete, readable narrative story from those photos — auto-writing an insight for any photo the giver left blank. The giver then previews that story rendered as a print-ready book in one of two physical formats, and can add it to the gift.

This is a **four-step wizard**: Photos → Insights (optional) → Story Preview → Print Preview. It is a genuine AI Services task end-to-end (unlike auto-chapter clustering) — both the per-photo insight generation and the full narrative generation require a language model, and per-photo insight generation additionally requires **vision/image-understanding capability**, not text-only generation.

---

## 2. Scope

### In scope
- Multi-photo upload/select UI scoped to a single gift draft (not the giver's own passport)
- Optional free-text "insight" per photo
- AI-generated insight for any photo left blank (requires vision-capable model — see Section 7, open question)
- AI-generated full narrative story combining all photos + all insights (user-provided and AI-generated)
- Editable story preview: regenerate, manual text edit, clear visual distinction between user-written and AI-written passages
- Print-ready preview in two formats:
  - **The Life Book** — 190mm × 190mm square, compact keepsake
  - **Passport Edition** — 297mm × 210mm, A4 landscape
- Page-spread preview UI (cover + inner pages, page navigation)
- Print specs summary (trim size, page count, cover type, price) and "Add to Gift" action

### Explicitly out of scope for this phase
- Actual print fulfillment/shipping integration (this spec covers generation and preview only; fulfillment is a separate, later integration)
- Editing photo order/layout by dragging pages around (v1: system-determined layout, no manual page reordering)
- Applying this same photo-story flow to a user's own (non-gift) passport — start with the gift flow only, generalize later if it proves valuable

---

## 3. Entity Changes Required

### New entity: `GiftDraft` (or extend existing gift entity if one exists from the current "Gift a Passport" implementation)
| Field | Type | Notes |
|---|---|---|
| `Id` | `Guid` (PK) | |
| `SenderUserId` | `Guid` (FK → User) | The gift-giver |
| `RecipientName` | `string` | Already exists in current Gift a Passport flow |
| `RecipientEmail` | `string?` | Already exists |
| `PassportTitle` | `string?` | Already exists |
| `PersonalMessage` | `string?` | Already exists |
| `GeneratedStoryId` | `Guid?` (FK → `GeneratedStory`) | Null until Step 3 completes |
| `PrintFormat` | `enum { LifeBook, PassportEdition }` | Selected in Step 4 |
| `Status` | `enum { DraftPhotos, DraftInsights, DraftStory, DraftPrint, Sent }` | Tracks wizard progress |

### New entity: `GiftPhoto`
| Field | Type | Notes |
|---|---|---|
| `Id` | `Guid` (PK) | |
| `GiftDraftId` | `Guid` (FK → GiftDraft) | |
| `Url` | `string` | Blob Storage reference |
| `UserInsight` | `string?` | Null if the giver left it blank |
| `AiGeneratedInsight` | `string?` | Populated by AI Services if `UserInsight` is null |
| `DisplayOrder` | `int` | For consistent story/page ordering |

### New entity: `GeneratedStory`
| Field | Type | Notes |
|---|---|---|
| `Id` | `Guid` (PK) | |
| `GiftDraftId` | `Guid` (FK → GiftDraft) | |
| `Title` | `string` | AI-generated headline |
| `BodyMarkdown` | `string` | Full story content, structured so paragraphs can be tagged by origin (see below) |
| `GeneratedAt` | `datetime` | |
| `RegenerationCount` | `int` | For analytics on how often users aren't satisfied with the first pass |

Each paragraph within `BodyMarkdown` needs to track whether it was derived from a user-written insight or fully AI-invented, to support the visual "Your words / AI-written" legend shown in the Story Preview screen. Simplest approach: store as structured JSON (array of `{ text, origin: "user" | "ai", sourcePhotoId }`) rather than flat markdown, if the editing/regeneration UI needs per-paragraph granularity.

---

## 4. Step-by-Step Flow

### Step 1 — Photo Selection
- Standard multi-upload to Blob Storage, same pattern as existing Memory Creation upload flow
- Each upload creates a `GiftPhoto` row with `UserInsight = null`
- No AI call yet — this step is pure upload/selection

### Step 2 — Insights (Optional)
- One card per uploaded photo, free-text field
- Saving is per-photo (autosave on blur, or a single "Continue" that persists all at once — either is fine, autosave is friendlier for a multi-photo form)
- No AI call yet — this step just persists `UserInsight` where provided
- UI shows a running count: "3 of 8 photos have an insight added"

### Step 3 — Story Generation (AI Services)
Triggered when the user proceeds from Step 2. This is where AI Services does two distinct jobs, in sequence:

```
FOR each GiftPhoto where UserInsight IS NULL:
    Call AI Services: generatePhotoInsight(photoUrl)
    → requires a vision-capable model (see Section 7 open question)
    → Store result in GiftPhoto.AiGeneratedInsight

THEN, once every photo has either UserInsight or AiGeneratedInsight:
    Call AI Services: generateGiftStory(photos: [{ url, insight, origin }])
    → text generation combining all photo captions/insights into one
      cohesive narrative with a title, opening, body paragraphs, and
      at least one pull-quote drawn from a user-provided insight
      (prefer user quotes over AI text for the pull-quote — it's more
      authentic and gives the user's own words visual prominence)
    → Store result as a new GeneratedStory row
```

- Show a loading state during generation (this will take several seconds minimum for 8 photos — two model calls in sequence, potentially more if insight generation is parallelized per-photo)
- Story Preview screen displays the result with the user/AI-origin legend

### Step 4 — Print Preview
- Read-only rendering of the `GeneratedStory`, laid out as book pages
- Format toggle switches the preview between the two physical dimensions (Section 5)
- No new AI call — this step is pure layout/rendering of existing content
- "Add to Gift" transitions `GiftDraft.Status` to reflect the story+format is finalized, then proceeds into the existing Gift a Passport checkout flow already built (`Pay & Send Gift`)

---

## 5. Print Format Specifications

| Format | Trim Size | Orientation | Notes |
|---|---|---|---|
| **The Life Book** | 190mm × 190mm | Square | Compact keepsake, matches the naming direction discussed for the flagship printed product line |
| **Passport Edition** | 297mm × 210mm (A4) | Landscape | Full A4, matches the "Passport Book" concept scoped earlier in the BRD (REL-3) |

**Implementation approach:**
- The preview UI (page-spread with left/right pages, cover, page navigation) can be built as an HTML/CSS layout component reused for both formats — only the container dimensions and type scale change between them, not the underlying page template structure.
- For actual print-ready output (not just on-screen preview), generate a PDF at print resolution (300 DPI) sized to the exact trim dimensions above, with appropriate bleed margins (typically 3mm) for the print vendor's requirements — confirm bleed spec with whichever print fulfillment partner is selected, since this varies by vendor.
- **Open decision:** PDF generation library/approach for .NET — options include a headless-browser HTML-to-PDF render (Puppeteer/Playwright via a Node sidecar, or similar) to reuse the same HTML/CSS layout as the on-screen preview, or a native .NET PDF library (e.g., QuestPDF) for tighter control over print-exact output. Headless-browser rendering is likely faster to build since it reuses the preview markup; a native library gives more precise typographic/print control. Recommend prototyping both with one sample story before committing.

---

## 6. Suggested Application-Layer Contracts

Following the existing vertical-slice pattern (`Features/GiftPassports/...`):

```csharp
// Commands
public record UploadGiftPhotoCommand(Guid GiftDraftId, Stream File) : IRequest<GiftPhotoDto>;
public record SetPhotoInsightCommand(Guid GiftPhotoId, string? Insight) : IRequest;
public record GenerateGiftStoryCommand(Guid GiftDraftId) : IRequest<GeneratedStoryDto>;
public record RegenerateGiftStoryCommand(Guid GiftDraftId) : IRequest<GeneratedStoryDto>;
public record SetPrintFormatCommand(Guid GiftDraftId, PrintFormat Format) : IRequest;

// AI Services internal contract (extends the existing AI Services interface
// used for itinerary generation and chapter story recaps)
public interface IGiftStoryAiService
{
    Task<string> GeneratePhotoInsight(string photoUrl);
    Task<GeneratedStoryContent> GenerateStory(IEnumerable<PhotoWithInsight> photos);
}
```

Keep `GenerateGiftStoryCommand`'s handler orchestration-only (loop photos needing AI insights, then call story generation) — the actual model-calling logic belongs behind `IGiftStoryAiService` so the AI provider can change without touching the Application layer, consistent with the existing AI Services isolation principle in the System Design Doc.

---

## 7. Open Questions for the Implementer

1. **Does the self-hosted Ollama setup include a vision-capable model?** Per-photo insight generation (Step 3) requires the model to actually interpret image content, not just generate text from a prompt. Confirm which model is deployed and whether it supports image input — if not, this either needs a different/additional model deployed, or Step 3's "AI will write this" behavior needs to fall back to generating insight text from available metadata only (photo date, location, chapter/event title) rather than true visual understanding. This materially affects output quality and is worth confirming before building the UI around an assumption of full vision capability.
2. **PDF generation approach** — headless-browser render vs. native .NET library (see Section 5). Needs a quick spike before committing.
3. **Print fulfillment partner** — out of scope for this spec, but the exact bleed/margin/color-profile requirements for the PDF export depend on whoever that vendor ends up being. Confirm before finalizing PDF export specifics.
4. **Pricing shown in Step 4** (₹2,499 in the mockup) — confirm this is the intended price for a gifted Life Book specifically, and whether it differs from the pricing on a user's own Passport Book.
5. **Regeneration limits** — should "Regenerate" in Step 3 be unlimited, or capped (e.g., 3 free regenerations) given it's a real per-call AI cost? Worth deciding before this ships, not after.

---

## 8. Acceptance Criteria

- [ ] Uploading photos with no insights provided still produces a complete, coherent story (full AI fallback path works end-to-end).
- [ ] A photo with a user-provided insight is never overwritten by AI-generated text — `UserInsight` always takes precedence over `AiGeneratedInsight` wherever both could apply.
- [ ] The Story Preview visually distinguishes user-sourced vs. AI-sourced content (matches the legend in the mockup).
- [ ] Switching between "The Life Book" and "Passport Edition" in Step 4 changes the preview's physical proportions without altering the story content itself.
- [ ] The exported/preview page count and layout adapt sensibly to very small (3-4 photos) and larger (25+ photos) sets — define a minimum photo count if fewer than some threshold produces a poor book (e.g., require at least 4-5 photos before allowing progression past Step 1, consistent with the ~4-5 photo threshold already used elsewhere in the product for chapter formation).
- [ ] Regenerating the story (Step 3) does not lose or alter any `UserInsight` values — only the generated narrative changes.
