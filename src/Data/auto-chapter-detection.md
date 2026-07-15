# Feature Spec: Automated Chapter Detection (Web v1.0)

**Status:** Ready for implementation
**Owner component:** `CelebrationPassports.Application` (Celebrations feature slice) + a new background worker
**Related docs:** `Celebration_Passport_System_Design_v1.docx` (Section 12), `Celebration_Passport_Entity_Reference_ERD_v0.1.docx` (PassportMoment, MomentMedium)

---

## 1. Summary

When a user manually uploads photos on the web app, the system should automatically group them into a Chapter (`PassportMoment`) once enough photos share a similar time and location — **with no confirmation prompt required from the user.** The user can always edit, merge, split, reassign, or delete the result afterward.

This is **not** an LLM/AI Services task. It's deterministic clustering logic (time + geo proximity) and belongs in the Celebration Service / Application layer, not the Ollama-backed AI Services component.

---

## 2. Scope

### In scope (this spec)
- Manual photo upload → `MomentMedium` created with `PassportMomentId = null` (unassigned)
- A debounced background job that clusters unassigned `MomentMedium` rows per Passport
- Automatic `PassportMoment` (Chapter) creation once a cluster meets threshold
- Direct `Active`/`Confirmed` status on creation (no approval gate)
- `Origin` field to distinguish `AiGenerated` vs `Manual` chapters
- Full manual edit/override of any auto-created chapter

### Explicitly out of scope (do not build yet)
- Any device photo-library scanning or auto-upload (mobile-only, later phase)
- Confirmation popups before a chapter is created
- AI-generated chapter titles or automatic cover-image selection
- Any call to the Ollama/AI Services component

---

## 3. Entity Changes Required

### `PassportMoment` — add fields
| Field | Type | Notes |
|---|---|---|
| `StatusId` | `Guid` (FK → `Status`) | If not already present. Set to the `Active`/`Confirmed` status value on creation — used by both manual and auto-created chapters in v1.0. |
| `Origin` | `enum { Manual, AiGenerated }` | New field. Defaults to `Manual` when a user explicitly creates a chapter; set to `AiGenerated` by the clustering job. |

### `MomentMedium` — confirm existing field behavior
| Field | Type | Notes |
|---|---|---|
| `PassportMomentId` | `Guid?` (nullable FK → `PassportMoment`) | Must be nullable. `null` = unassigned/pending clustering. This is the query target for the clustering job. |

> If `PassportMomentId` is not currently nullable in the existing schema, this is a required migration before implementation proceeds.

---

## 4. Clustering Algorithm (deterministic, no AI)

**Input:** all `MomentMedium` rows where `PassportMomentId IS NULL`, scoped to a single `PassportId`.

```
FOR a given PassportId:
  1. Load all unassigned MomentMedium rows for this Passport
     (join to get CreatedAt/EXIF timestamp and geo coordinates if present)
  2. Sort by timestamp
  3. Group into candidate clusters where consecutive items are within:
     - TIME_WINDOW (default: same calendar day, configurable)
     - GEO_RADIUS (default: ~2km, only applied if both items have coordinates;
       skip geo check if coordinates are missing — fall back to time-only clustering)
  4. FOR each candidate cluster:
       IF cluster.size >= CLUSTER_THRESHOLD (default: 5, min 4):
         a. Create new PassportMoment:
              - PassportId = current passport
              - Title = date-range label, e.g. "Mar 12–14" (derived, not AI-generated)
              - StatusId = Active/Confirmed
              - Origin = AiGenerated
         b. Update each MomentMedium.PassportMomentId in the cluster to the new PassportMoment.Id
       ELSE:
         Leave cluster's MomentMedium rows unassigned (PassportMomentId stays null)
         — they'll be re-evaluated on the next debounced run as more uploads arrive
```

**Config values to expose (appsettings or a constants file), not hardcode inline:**
- `ChapterClustering:TimeWindowHours` (default 24)
- `ChapterClustering:GeoRadiusKm` (default 2)
- `ChapterClustering:MinPhotosPerChapter` (default 5, floor 4)
- `ChapterClustering:DebounceMinutes` (default 2)

---

## 5. Debounce / Trigger Mechanism

**Requirement:** Do not run clustering per-upload. Wait until uploads for a given Passport have paused for `DebounceMinutes` before running.

**Recommended implementation pattern** (cancel-and-reschedule debounce), since Redis is already in the architecture:

1. On each successful photo upload, the API handler:
   - Writes/updates a Redis key: `debounce:cluster:{passportId}` → value = a job/schedule reference, TTL = `DebounceMinutes`
   - If a previously scheduled clustering job for this `passportId` exists (tracked via that Redis key), **cancel/reschedule it** rather than letting duplicates queue up
   - Schedules a new delayed job for `now + DebounceMinutes`

2. This requires a background job scheduler that supports delayed + cancelable jobs. **Open decision — confirm before implementation:** does the project already have Hangfire, Quartz.NET, or a custom `BackgroundService` queue? None was visible in the shared solution structure. Hangfire is the common choice for ASP.NET Core Clean Architecture projects and pairs well with PostgreSQL (it can use PostgreSQL as its job store, avoiding a new infra dependency).

3. When the delayed job fires, it runs the clustering algorithm (Section 4) for that single `passportId` only — not a global sweep of all passports.

**Fallback (simpler, if a job scheduler isn't set up yet):** a periodic `BackgroundService` that runs every N minutes and processes any Passport with unassigned media whose most recent upload is older than `DebounceMinutes`. Less precise than true debounce, but requires no new infrastructure. Acceptable for v1.0 if Hangfire isn't already planned.

---

## 6. Suggested Application-Layer Contract

Following the existing `Features/Celebrations` vertical-slice pattern:

```csharp
// Application/Features/Celebrations/Commands/ClusterUnassignedMedia/
public record ClusterUnassignedMediaCommand(Guid PassportId) : IRequest<ClusterResult>;

public record ClusterResult(int ChaptersCreated, int MediaAssigned, int MediaStillUnassigned);

public class ClusterUnassignedMediaHandler
    : IRequestHandler<ClusterUnassignedMediaCommand, ClusterResult>
{
    // 1. Query unassigned MomentMedium for PassportId
    // 2. Run clustering algorithm (Section 4) — consider extracting as
    //    a pure, unit-testable domain service, e.g. IMediaClusteringService,
    //    so the grouping logic has no EF/DB dependency and is easy to test
    // 3. Create PassportMoment rows, update MomentMedium FKs
    // 4. Return summary result (useful for logging/analytics on Origin usage)
}
```

Keep the actual grouping math (time/geo bucketing) in a separate, pure class — e.g. `Domain/Services/MediaClusteringService` or similar — so it can be unit tested with plain in-memory data and no database.

---

## 7. Acceptance Criteria

- [ ] Uploading fewer than the threshold number of matching photos does **not** create a chapter; media remains unassigned.
- [ ] Uploading a batch that crosses the threshold creates exactly one chapter containing all matching media.
- [ ] Uploading 30 photos in rapid succession triggers clustering **once**, after the debounce window, not multiple times.
- [ ] A chapter created this way has `Origin = AiGenerated` and `StatusId` = Active/Confirmed — visible and usable immediately, no approval step.
- [ ] The user can rename, merge, split, reassign media out of, or delete an auto-created chapter with no special-casing based on `Origin`.
- [ ] Photos with no geo data still cluster correctly using time-only proximity.
- [ ] Two unrelated small clusters (e.g., 2 photos each, different days) do **not** merge into one chapter.
- [ ] Manually created chapters (`Origin = Manual`) are never touched or reassigned by the clustering job.

---

## 8. Open Questions for the Implementer

1. Does the project have a background job framework yet (Hangfire/Quartz/custom)? This blocks the debounce implementation choice in Section 5.
2. Confirm `PassportMomentId` on `MomentMedium` is nullable in the actual schema — migration needed if not.
3. Confirm exact `Status` values available (is there already an `Active`/`Confirmed` row to reference, or does one need seeding?).
4. Should `GeoRadiusKm` fall back gracefully per-photo (some photos in a batch have GPS, some don't), or should missing GPS on any photo in a candidate cluster disqualify geo-matching for the whole cluster? (Spec above assumes per-photo graceful fallback — confirm this is desired.)
