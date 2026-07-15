# Feature Spec: v1.1 / v2.0 Feature Backlog

**Status:** Planning-ready, not build-ready — each feature needs its own scoping pass before implementation
**Related docs:** `Celebration_Passport_BRD_v1.docx` (Section 12), `Celebration_Passport_Entity_Reference_ERD_v0.1.docx`, `Celebration_Passport_System_Design_v1.docx`

> **How to use this file with Claude Code:** this is a backlog overview, not a single implementation spec (unlike `auto-chapter-detection.md`). Point Claude Code at **one feature at a time** — e.g. *"Read the 'Time-Capsule Messages' section of docs/features/feature-backlog-v1.1.md and draft the Application-layer command/handler for it."* Do not ask it to implement multiple features from this file in one pass.

---

## How These Fit the Existing Architecture

All features below build on entities already documented in the Entity Reference (`Passport`, `PassportMember`, `PassportMoment`, `MomentMedium`, `Status`) or extend the Application layer's existing `Features/Celebrations` and `Features/Passports` vertical slices. None of them require a new microservice — they're new commands/queries/entities within the existing Celebration Service, except where noted (AI Services calls are explicitly marked).

---

## PRESERVE

### 1. Import Existing Memories
**Priority:** Should · **Target:** v1.1
**Problem it solves:** A new passport starts empty — the single biggest onboarding drop-off risk.
**Approach:** A one-time import flow accepting a Google Photos album export (JSON + media) or a WhatsApp chat export (.zip with media + `_chat.txt`). Parse timestamps/media from the export, create `MomentMedium` rows as `PassportMomentId = null` (unassigned), and let the existing auto-chapter clustering job (see `auto-chapter-detection.md`) organize them normally — no special-cased import logic needed beyond parsing.
**New entity work:** None required beyond existing `MomentMedium`. Consider an `ImportJob` entity to track long-running import status/progress if imports are processed asynchronously (likely, given large exports).
**Open question:** WhatsApp export parsing is fragile (format varies by OS/version) — scope a narrow supported format first rather than a general parser.

### 2. Voice Note Transcription
**Priority:** Should · **Target:** v1.1
**Approach:** When a voice `MomentMedium` (MediaType = VoiceNote) is uploaded, asynchronously call AI Services to transcribe it (this **is** an appropriate AI Services task — speech-to-text, not clustering logic) and store the transcript.
**New entity work:** Add `TranscriptText` (nullable string) to `MomentMedium`, or a related `MediaTranscript` table if you want transcript versioning/edit history.
**Dependency:** Requires a speech-to-text capable model available to AI Services — confirm whether the self-hosted Ollama setup supports this or whether a separate model/service is needed.

### 3. Scan Physical Keepsakes
**Priority:** Could · **Target:** v1.1
**Approach:** Standard photo upload flow, tagged with a `SourceType = ScannedPhysical` on `MomentMedium`. OCR runs asynchronously via AI Services, populating the same `TranscriptText`-style field used for voice notes (reuse, don't duplicate the pattern).
**Open question:** Confirm OCR model choice; this is a distinct capability from voice transcription and may need a different provider.

### 4. Memory Map
**Priority:** Should · **Target:** v1.1
**Approach:** Read-only feature — query all `MomentMedium`/`PassportMoment` rows with non-null geo coordinates for a Passport, return as a list of map pins. Frontend-only effort once the query exists (a new `GetPassportMemoryMapQuery` in `Features/Celebrations`); no new entities.
**Dependency:** Requires geo coordinates to be reliably captured at upload time (EXIF extraction) — confirm this is already happening for the auto-chapter clustering feature and reuse the same extraction logic.

### 5. Budget: Actual vs. Planned
**Priority:** Could · **Target:** v1.1
**Approach:** The BRD already specifies a budget planner (Plan pillar, PLAN-5) and expense logging (Preserve pillar, PRES-4). This feature is a query/reporting layer joining the two — no new entities, just a `GetBudgetVarianceQuery` summing logged expenses against the planned budget for a given Plan/Passport.

---

## RELIVE

### 6. Time-Capsule Messages
**Priority:** Should · **Target:** v1.1
**Approach:** New entity, e.g. `TimeCapsuleMessage`: `PassportId` (FK), `AuthorUserId` (FK), `Content` (string), `UnlockDate` (datetime), `IsUnlocked` (bool, computed or set by a scheduled check), `CreatedAt`.
A scheduled job (daily is sufficient — no debounce needed here, unlike chapter clustering) checks for messages where `UnlockDate <= now AND IsUnlocked = false`, flips the flag, and triggers a notification.
**Dependency:** Needs the notification mechanism flagged as a future consideration in the System Design Doc (Section 11) — a good forcing function to actually build that now rather than later.

### 7. Private Annual Recap
**Priority:** Should · **Target:** v1.1
**Approach:** This **is** an AI Services task (narrative generation via Ollama), reusing the same story-recap capability already scoped for individual chapters — just called with a full year's Moments as input instead of one chapter's. No new entity required if recaps are generated on-demand and not persisted; add a `PassportRecap` entity only if you want to cache/store generated recaps rather than regenerating each time they're viewed.

### 8. Then vs. Now
**Priority:** Could · **Target:** v1.2
**Approach:** Query-only feature once Memory Map (#4) exists — find `PassportMoment`/`MomentMedium` pairs within a small geo-radius of each other but in different years. No new entities.

### 9. Ask Your Passport
**Priority:** Could · **Target:** v1.2
**Approach:** Two phases, don't conflate them:
- **v1.1-friendly version:** plain full-text search (PostgreSQL `tsvector`/full-text search, or a simple `ILIKE` query to start) across `PassportMoment.Title`/`Description` and `MomentMedium.TranscriptText`. No AI required.
- **v1.2 stretch:** natural-language query via AI Services (Ollama), translating a question into the same search query above. Treat this as a thin layer on top of the v1.1 search, not a replacement for it.

---

## CELEBRATE

### 10. Scoped Family Sharing
**Priority:** Should · **Target:** v1.1
**Approach:** Extends `PassportMember`/`PassportMemberRole` (already in the schema) with a new concept: chapter-scoped access rather than passport-wide. Add a `PassportMomentContributor` join entity: `PassportMomentId` (FK), `UserId` (FK), `Role` (e.g., Contributor). Access-control checks in the Authentication/Celebration Service need a new rule: "full member OR scoped contributor to this specific chapter."
**Open question:** Confirm this doesn't conflict with the existing `PassportMemberRole` design — worth a short design review before building, since it adds a second, narrower permission dimension alongside the existing passport-wide one.

### 11. Digital Guestbook Mode
**Priority:** Could · **Target:** v1.1
**Approach:** A public, tokenized link (no login required) scoped to one `PassportMoment`, allowing an anonymous or lightly-identified guest to submit one photo + one note. Submissions land as `MomentMedium` + a `Wish`/comment record, likely in a `PendingReview` state the passport owner approves before it's visible — unlike the auto-chapter flow, this one *should* have a review gate, since it's unvetted external input.

### 12. Chapter Soundtrack
**Priority:** Could · **Target:** v1.2
**Approach:** Add `SongTitle`, `SongArtist`, `ExternalLinkUrl` (nullable strings) to `PassportMoment` or a small related `ChapterSoundtrack` entity. Deliberately no audio hosting or streaming API integration — just metadata and an outbound link — to avoid licensing complexity entirely.

---

## PLAN

### 13. Someday List
**Priority:** Should · **Target:** v1.1
**Approach:** New entity `SomedayIdea`: `PassportId` (FK), `Title`, `Notes`, `CreatedByUserId`, `CreatedAt`, `ConvertedToPlanId` (nullable FK, set when an idea graduates into a real dated Plan). Simple CRUD, no AI involvement in v1.1.

### 14. Gift a Passport
**Priority:** Should · **Target:** v1.1
**Approach:** This is a commerce feature more than a data-model feature — a purchase flow that creates a new `Passport` in a "gifted, awaiting claim" state and generates a redemption link/code. Needs a payment provider decision (not yet made anywhere in existing docs) before scoping further. Flag this as the one item in this backlog most likely to need its own dedicated spec before implementation, given payment/legal surface area.

### 15. Milestone Calendar Sync
**Priority:** Could · **Target:** v1.1
**Approach:** Generate a standard `.ics` feed URL per Passport (or per user) covering upcoming milestones/countdowns. Read-only, no new entities — a formatting layer over existing milestone data.

---

## CROSS-PILLAR

### 16. Single-Chapter Print Card
**Priority:** Should · **Target:** v1.1
**Approach:** A smaller-scope version of the existing Passport Book ordering flow (already scoped in the BRD, REL-3) — same print/fulfillment pipeline, just triggered per-chapter instead of per-year. Build the Passport Book pipeline first (v1.0 requirement); this is a parameterization of it, not a separate system.

---

## Suggested Build Order (if picking up this backlog)

1. **Someday List** and **Milestone Calendar Sync** — simplest, no dependencies, good warm-up items
2. **Import Existing Memories** — highest impact on activation, worth prioritizing even though it's more complex
3. **Time-Capsule Messages** and **Memory Map** — meaningfully increase reasons to return to the app
4. **Scoped Family Sharing** — needed before Digital Guestbook Mode can be built on top of it
5. Everything else, roughly in the priority/target order shown above

## Open Questions Across the Whole Backlog

- Notification infrastructure (push/email) isn't built yet — Time-Capsule Messages and several others depend on it existing.
- Payment provider hasn't been chosen anywhere in the existing docs — blocks Gift a Passport and is likely needed for the Passport Book itself.
- Confirm whether AI Services' Ollama setup can handle speech-to-text (#2) and OCR (#3), or whether those need a separate model/provider — this changes the AI Services component's scope beyond what's currently documented (itinerary + story recaps only).
