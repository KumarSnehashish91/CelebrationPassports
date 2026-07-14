# Celebration Passports — Database Schema Reference (Draft v7)

Companion doc to the ERD. This version is a structural rebuild driven by
walking through the actual application flow (Login → Dashboard → Events
→ Stories → Chapters → Passport) rather than working from feature
requests in isolation. 31 tables.

## Version summary (abridged — v1–v6 covered incremental additions)

- **v1–v3** — initial full draft; renamed tables for clarity; soft
  delete; ownership transfer; finalized MVP enums.
- **v4** — added `CalendarEvents`, `MilestoneDefinitions` +
  `PassportMilestoneProgress`, `PassportStamps`, `ActivityLog` to match
  features already visible in the beta UI.
- **v5** — build order documented; surfaced and resolved a circular FK
  dependency (cover images pointing at `Media`, which traces back to the
  entity that has the cover image).
- **v6** — added `UserProfiles` and `PassportBooks` +
  `PassportBookChapters`, confirmed via the two live theme designs.
- **v7** *(this version)* — walked the actual screen-by-screen app flow
  and found the object model didn't match it. Three structural changes:
  1. **`Memories` is removed entirely.** Chapters attach directly to
     `Media` — there is no intermediate "memory" record. ("Memory
     Highlights" in the UI is just a curated recent-photos query over
     `Media`, not a table.)
  2. **A `Story` layer is inserted between `Passport` and `Chapter`.**
     `Passport` → `Story` → `Chapter` → `Media`. A Story is what AI
     builds from a batch of photos (e.g. "Manali Trip" containing a
     "Road Trip" chapter and an "In Manali" chapter).
  3. **`Event` is a new, separate concept from `Story`** — the *plan*
     (dates, place, itinerary, status), not the *record*. `Trips` is
     retired; `Event` supersedes it. `CalendarEvents` now belongs to
     `Event` (itinerary line-items like "Dinner at 7pm"), not directly to
     `Passport`. An `Event` optionally produces a `Story` once completed
     (`Events.story_id`) — but a `Story` can also exist with **no**
     `Event` behind it at all, for spontaneous, unplanned photo uploads.
     To make that work symmetrically, `Story` carries its own
     `place_id`/`start_date`/`end_date`, AI-inferred from its Chapters'
     photos either way — so trip tracking doesn't depend on whether
     anyone planned ahead.
  4. **`Comments` and `Reactions`** now attach to *either* `Chapter` or
     `Media` (and `Reactions` can also attach to a `Comment`) — someone
     can react to a whole chapter or to one specific photo in it.

## The core model, in plain terms

- **`Event`** — the plan. "We're going to Manali, here's the itinerary."
  Has a lifecycle (`Draft` → `Upcoming` → `Ongoing` → `Completed`).
  Optional — not every trip starts as a plan.
- **`CalendarEvents`** — the itinerary line-items inside an `Event`
  ("Dinner at 7pm", "Museum at 10am").
- **`Story`** — the record. What actually happened, built from uploaded
  photos, whether or not an `Event` preceded it. Groups `Chapter`s.
- **`Chapter`** — a segment of a Story ("Road Trip", "In Manali"). Has
  its own `place_id`/`event_date`, since a Story's chapters can span
  multiple places and days.
- **`Media`** — photos/videos, attached directly to a `Chapter`
  (nullable — upload-first, sort-into-a-chapter-later still works).
- **`Comments`/`Reactions`** — attach to a `Chapter` or a `Media` item;
  `Reactions` can additionally attach to a `Comment`.

## Entities, fields, and constraints

*(Only new or structurally changed entities are shown in full detail
below. Unchanged entities from v6 — `Users`, `UserProfiles`,
`UserSessions`, `LoginHistory`, `Notifications`, `SubscriptionPlans`,
`UserSubscriptions`, `Payments`, `Passports`, `PassportOwnershipHistory`,
`PassportPeople`, `PassportInvitations`, `PassportShares`, `Places`,
`Categories`, `PassportBooks`, `PassportBookChapters`, `MediaVariants`,
`WishlistItems`, `MilestoneDefinitions`, `PassportMilestoneProgress`,
`PassportStamps`, `ActivityLog`, `DailyStatistics` — keep their v6 field
definitions; see the diagram for the complete current field list on
every table.)*

### Events *(new in v7 — replaces Trips)*
| field | type | notes |
|---|---|---|
| id | uuid PK | |
| passport_id | uuid FK → Passports | not null |
| title | string | not null |
| status | enum | `Draft`, `Upcoming`, `Ongoing`, `Completed` |
| start_date | date | not null |
| end_date | date, nullable | |
| place_id | uuid FK → Places, nullable | |
| notes | string, nullable | arrangements, what's needed, plan details |
| story_id | uuid FK → Stories, nullable | set once the event is completed and its Story is generated |
| created_by | uuid FK → Users | |
| created_at | timestamp | |
| is_deleted | bool | |
| deleted_on | timestamp | |
| deleted_by | uuid FK → Users, nullable | |

### CalendarEvents *(re-parented in v7)*
| field | type | notes |
|---|---|---|
| id | uuid PK | |
| event_id | uuid FK → Events | not null — **changed from `passport_id`/`chapter_id`** |
| created_by | uuid FK → Users | |
| title | string | not null |
| location | string, nullable | |
| event_time | timestamp | not null |
| color_tag | string | |

### Stories *(new in v7)*
| field | type | notes |
|---|---|---|
| id | uuid PK | |
| passport_id | uuid FK → Passports | not null |
| title | string | |
| place_id | uuid FK → Places, nullable | AI-inferred from Chapters/Media, user-editable |
| start_date | date, nullable | AI-inferred |
| end_date | date, nullable | AI-inferred |
| cover_media_id | uuid FK → Media, nullable | deferred constraint — see Build Order |
| display_order | int | |
| created_at | timestamp | |
| is_deleted | bool | |
| deleted_on | timestamp | |
| deleted_by | uuid FK → Users, nullable | |

**No `source_event_id` on this table** — the link lives on `Events.story_id`
instead (one direction only, avoids a redundant bidirectional FK).

### Chapters *(re-parented in v7)*
| field | type | notes |
|---|---|---|
| id | uuid PK | |
| story_id | uuid FK → Stories | not null — **changed from `passport_id`**; `trip_id` removed |
| category_id | uuid FK → Categories | |
| place_id | uuid FK → Places, nullable | can differ from the parent Story's place |
| cover_media_id | uuid FK → Media, nullable | deferred constraint |
| title | string | not null |
| event_date | date | not null |
| display_order | int | |
| is_deleted | bool | |
| deleted_on | timestamp | |
| deleted_by | uuid FK → Users, nullable | |

### Media *(re-parented in v7)*
| field | type | notes |
|---|---|---|
| id | uuid PK | |
| chapter_id | uuid FK → Chapters, **nullable** | null = uploaded but not yet sorted into a chapter |
| uploaded_by | uuid FK → Users | **changed from `memory_id`** — Media now attaches directly to Chapter |
| url | string | |
| type | enum | `Photo`, `Video`, `Audio`, `Document` |
| is_deleted | bool | |
| deleted_on | timestamp | |
| deleted_by | uuid FK → Users, nullable | |

### Comments *(changed target in v7)*
| field | type | notes |
|---|---|---|
| id | uuid PK | |
| chapter_id | uuid FK → Chapters, nullable | set if commenting on the whole chapter |
| media_id | uuid FK → Media, nullable | set if commenting on one photo |
| user_id | uuid FK → Users | |
| text | string | |
| created_at | timestamp | |
| is_deleted | bool | |
| deleted_on | timestamp | |
| deleted_by | uuid FK → Users, nullable | |

**Constraint:** exactly one of `chapter_id` / `media_id` set.

### Reactions *(changed target in v7)*
| field | type | notes |
|---|---|---|
| id | uuid PK | |
| chapter_id | uuid FK → Chapters, nullable | |
| media_id | uuid FK → Media, nullable | |
| comment_id | uuid FK → Comments, nullable | |
| user_id | uuid FK → Users | |
| type | enum | `heart`, `laugh`, `wow`, `sad` |

**Constraint:** exactly one of `chapter_id` / `media_id` / `comment_id`
set — three possible targets now, not two.

## What was removed in v7

- **`Memories`** — fully removed. Everything it did (a "moment" with a
  caption, linked to a chapter, holding AI fields) is now either on
  `Chapter` (the caption/story context) or simply not modeled as a
  separate row (individual photos are just `Media` rows).
- **`Trips`** — retired. `Events` covers the same ground (passport,
  place, dates) plus status and itinerary, which `Trips` never had.

## Design decisions worth knowing the reasoning behind

*(Carried forward from v1–v6, still true — soft delete exceptions for
`Payments`/`Reactions`/`Places`/`Categories`; `PassportShares` uses
`RevokedAt`; `Passports.status` has no `Deleted` value; `PassportStamps`
is permanent, not a live count; `ActivityLog.subject_id` is intentionally
not a real FK.)*

New in v7:

- **`Story.place_id`/`start_date`/`end_date` exist independently of
  `Event`.** This was a deliberate fix after realizing the original v7
  draft only tracked trip dates/place on `Event` — which meant a
  spontaneous, unplanned trip (no Event ever created) would have no
  aggregate place/date data at all, only scattered per-Chapter data.
  Now AI populates these on `Story` directly from the Chapters/Media it
  processes, regardless of whether an `Event` came first.
- **`Events.story_id` is one-directional.** No matching `source_event_id`
  on `Story` — a Story either has exactly one Event pointing at it, or
  none. No need for a second FK to express the same relationship.
- **Comments/Reactions constraint is now three-way, not two-way** — a
  reaction can land on a Chapter, a Media item, or a Comment, but only
  ever exactly one of the three per row.

## Build order (updated for v7)

The same circular-dependency shape from v5 still applies, just routed
through `Story` now instead of directly through `Chapter`:

**Passports → Media → Chapters → Stories → Passports**

Same fix as before: create the `cover_media_id`/`avatar_media_id`
columns without FK constraints first, add the real constraints in one
final migration once every table in the cycle exists.

**Phase 1 — Identity:** `User` → `UserProfile` *(avatar_media_id, no FK
yet)* → `UserSession` → `UserLoginHistory` → `Notification`

**Phase 2 — Lookup:** `Category` → `Place` → `SubscriptionPlan` →
`MilestoneDefinitions`

**Phase 3 — Passport core:** `Passport` *(cover_media_id, no FK yet)* →
`PassportPeople` → `PassportInvitation` → `PassportShare` →
`PassportOwnershipHistory`

**Phase 4 — Events:** `Event` *(story_id, no FK yet)* → `CalendarEvent`

**Phase 5 — Story:** `Story` *(place_id ok — Places already exists;
cover_media_id, no FK yet)* → `Chapter` *(cover_media_id, no FK yet)* →
`Media` → `MediaVariant` → `PassportBook` *(cover_media_id, no FK yet)*
→ `PassportBookChapters` → `Comment` → `Reaction`

**Phase 6 — Close the loop:** add the deferred FK constraints now that
`Media`, `Chapters`, and `Stories` all exist: `UserProfile.avatar_media_id`,
`Passport.cover_media_id`, `Story.cover_media_id`,
`Chapter.cover_media_id`, `PassportBook.cover_media_id`,
`Event.story_id`.

**Phase 7 — Insights:** `ActivityLog` → `DailyStatistic` →
`WishlistItem` → `PassportStamp`

**Phase 8 — Commerce:** `UserSubscription` → `Payment`

## Remaining open items

1. **Display name for a soft-deleted user** inside someone else's
   passport — still a UI decision, not a schema one.
2. **`PassportStamps.source_chapter_id`** — now that `Story` also
   carries `place_id`, consider whether stamps should source from
   `Story` instead of `Chapter` (one stamp per trip vs. one stamp per
   granular location within a trip). Not changed in this version — worth
   a product decision.
3. **`ActivityLog` verb list** is illustrative, not final.
4. **Milestone catalog content** — the ~50 milestones still need to be
   defined and seeded.
5. **What happens to `CalendarEvents` when their parent `Event` is
   marked `Completed`** — do they stay as a historical record of the
   plan (my assumption, matches "we will show how event was planned"),
   or do they get archived/hidden once the Story exists? Assumed: they
   stay, unchanged, permanently — the completed Event screen shows both
   the original itinerary and the resulting Story side by side.
