# Implementation Backlog — Recommended Build Order

**Status:** Living document — update as items complete or priorities shift
**Purpose:** A single, ordered sequence across every feature spec produced so far, so Claude Code (or any implementer) knows what to build next and why, including what blocks what.

---

## How to Use This File

Point Claude Code at **one phase at a time**, not the whole file at once. Each phase lists the spec file(s) it draws from. Do not start a phase whose blocking decisions (marked ⛔) aren't resolved yet — implementing around an unresolved blocker usually means redoing the work once the decision lands.

---

## Phase 0 — Blocking Decisions (resolve before writing more feature code)

These aren't code tasks — they're decisions that multiple downstream specs depend on. Resolving them late means rework.

| # | Decision | Blocks |
|---|---|---|
| 1 | ⛔ Background job framework (Hangfire vs. custom `BackgroundService`) | Auto-chapter debounce (`auto-chapter-detection.md`), scheduled gift delivery (`gift-message-schedule-claim.md`) |
| 2 | ⛔ Payment provider (Razorpay recommended — see prior discussion) | Gift a Passport checkout, Passport Book / Life Book purchase |
| 3 | ⛔ Confirm vision-capable model available in Ollama deployment | Photo insight generation (`gift-story-print-preview.md`, `ai-model-strategy.md`) |
| 4 | ⛔ PDF generation approach (headless-browser render vs. native .NET library) | Print-ready export (`gift-story-print-preview.md`, Section 5) |
| 5 | Trademark/naming resolution (Stampfolio or final choice) | Not a code blocker, but blocks further public-facing branding work |
| 6 | Confirm `PersonalMessage` field already exists on current gift entity | Avoids duplicate field in `gift-message-schedule-claim.md` |

---

## Phase 1 — Core v1.0 Completion

Already partially built per your shared screenshots (Dashboard, Gift a Passport basic flow, Story reading screen). Close out what's left.

1. **Automated Chapter Detection** — `auto-chapter-detection.md`
   - Debounced clustering job, `Origin` field, no-confirmation-popup state model
   - Depends on: Phase 0 #1
2. **Entity Reference verification** — confirm `Celebration_Passport_Entity_Reference_ERD_v0.1.docx` against real entity source code, move status from Draft to Verified

---

## Phase 2 — AI Services Foundation

Do this before building more AI-dependent features on top of a shaky base.

1. **AI Model Strategy, Stage 1** — `ai-model-strategy.md`
   - Confirm/select vision-capable model (Phase 0 #3)
   - Prompt engineering pass for insight generation (system prompt + few-shot examples)
2. **AI Model Strategy, Stage 2 (trip planning only)** — `ai-model-strategy.md`
   - Build the RAG layer: curated destination data store, retrieval-at-generation-time for itinerary suggestions
   - This directly improves the existing itinerary generation feature — no new UI needed, quality improvement only

---

## Phase 3 — Gift Passport: Photo Story & Print

The photo-to-book flow, layered onto the existing Gift a Passport feature.

1. **Gift Story & Print Preview** — `gift-story-print-preview.md`
   - 4-step wizard: Photos → Insights → Story → Print Preview
   - Depends on: Phase 0 #3, Phase 0 #4, Phase 2 #1
2. **Gift Message, Scheduled Delivery & Claim-to-Continue** — `gift-message-schedule-claim.md`
   - Step 5 (personal message + delivery timing), public claim page, post-claim first-run onboarding
   - Depends on: Phase 0 #1, Phase 0 #2 (payment must complete before scheduled-delivery logic is meaningful), Phase 3 #1 (claim flow inserts the generated story as the recipient's first chapter)

---

## Phase 4 — v1.1 Feature Backlog

Full list and detail in `feature-backlog-v1.1.md`. Recommended sub-order (from that doc's own "Suggested Build Order" section, repeated here for a single source of truth):

1. **Someday List** — simplest, no dependencies
2. **Milestone Calendar Sync** — simplest, no dependencies
3. **Import Existing Memories** — higher complexity, highest activation impact
4. **Time-Capsule Messages** — needs notification infrastructure (see that doc's open questions)
5. **Memory Map** — needs reliable geo-extraction at upload (likely already built for auto-chapter clustering — reuse)
6. **Scoped Family Sharing** — needed before Digital Guestbook Mode
7. Remaining items (Voice Note Transcription, Private Annual Recap, Then vs. Now, Ask Your Passport, Digital Guestbook Mode, Chapter Soundtrack, Gift a Passport [now superseded by Phase 3's deeper version], Single-Chapter Print Card) — prioritize by the Should/Could + v1.1/v1.2 tags already assigned in that doc

---

## Phase 5 — AI Model Strategy, Stage 3+ (post-launch only)

**Do not start this phase until Phase 3 and Phase 4 have real user activity.** There is no useful data to act on before then.

1. Add opt-in consent for generation-vs-edit data collection (Privacy Policy update required first)
2. Build the collection pipeline (store AI output + user-edited final version pairs)
3. Only after a specific, provable quality gap emerges that prompting can't fix: scope a LoRA/QLoRA fine-tuning pass

---

## Explicitly Not Sequenced Yet (raised in conversation, not yet spec'd)

These were discussed as good ideas but don't have a spec file yet. Write a spec (following the pattern of the files above) before adding them to a phase:

- Group gifting (multiple contributors to one gift)
- QR code inside the printed book linking back to the digital version
- Corporate/bulk gifting
- "Remind me to gift again next year"
- Gift status visibility for the sender ("Opened 2 days ago")

---

## Change Log

| Date | Change |
|---|---|
| Initial version | Consolidated from all specs produced to date: auto-chapter-detection, feature-backlog-v1.1, gift-story-print-preview, gift-message-schedule-claim, ai-model-strategy |
