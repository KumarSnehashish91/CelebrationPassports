using CelebrationPassports.Application.Stories.Configuration;
using CelebrationPassports.Application.Stories.DTOs;
using CelebrationPassports.Application.Stories.Interfaces;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Enums;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using MediaEntity = CelebrationPassports.Persistence.Entities.Media;

namespace CelebrationPassports.Application.Stories.Services;

// Implements auto-chapter-detection.md (src/Data), adapted to this project's actual
// schema: the spec's PassportMoment/MomentMedium map to this codebase's Chapter/Media,
// and its Status/Origin fields already exist here as ChapterStatus/ChapterSource — no
// entity changes were needed for those. Unlike the spec's assumption, Media has no
// direct PassportId (only Chapter does), so unassigned media is scoped by uploader
// instead — same pattern as TripDetectionService — and a new Chapter lands in that
// user's first Passport.
//
// Deliberately simple/heuristic where the spec leaves room for it (place naming, "same
// place" reuse radius) — same "honest about what's real" convention as
// TripDetectionService, which this mirrors for the Place-resolution part.
public class AutoChapterClusteringService : IAutoChapterClusteringService
{
    private const string DefaultCategoryName = "Everyday";

    // "Same place" reuse radius when resolving/creating a Place for a new cluster —
    // mirrors TripDetectionService.SamePlaceKm.
    private const double SamePlaceReuseKm = 5;

    private readonly IGenericRepository<MediaEntity> _mediaRepository;
    private readonly IGenericRepository<Place> _placeRepository;
    private readonly IGenericRepository<Category> _categoryRepository;
    private readonly IPassportRepository _passportRepository;
    private readonly IChapterRepository _chapterRepository;
    private readonly IStoryRepository _storyRepository;
    private readonly IMediaClusteringService _clusteringService;
    private readonly IOptionsMonitor<ChapterClusteringOptions> _options;
    private readonly IUnitOfWork _unitOfWork;

    public AutoChapterClusteringService(
        IGenericRepository<MediaEntity> mediaRepository,
        IGenericRepository<Place> placeRepository,
        IGenericRepository<Category> categoryRepository,
        IPassportRepository passportRepository,
        IChapterRepository chapterRepository,
        IStoryRepository storyRepository,
        IMediaClusteringService clusteringService,
        IOptionsMonitor<ChapterClusteringOptions> options,
        IUnitOfWork unitOfWork)
    {
        _mediaRepository = mediaRepository;
        _placeRepository = placeRepository;
        _categoryRepository = categoryRepository;
        _passportRepository = passportRepository;
        _chapterRepository = chapterRepository;
        _storyRepository = storyRepository;
        _clusteringService = clusteringService;
        _options = options;
        _unitOfWork = unitOfWork;
    }

    public async Task<ClusterResult> ClusterUnassignedMediaForUserAsync(Guid userId)
    {
        var unassigned = await _mediaRepository.FindAsync(m =>
            m.UploadedBy == userId && m.ChapterId == null && m.PendingClustering && !m.IsDeleted);

        if (unassigned.Count == 0)
        {
            return new ClusterResult(0, 0, 0);
        }

        var passports = await _passportRepository.GetForUserAsync(userId);
        var passport = passports.FirstOrDefault();

        if (passport is null)
        {
            return new ClusterResult(0, 0, unassigned.Count);
        }

        var options = _options.CurrentValue;

        var clusterable = unassigned
            .Select(m => new ClusterableMedia(m.Id, m.CapturedAt ?? m.UploadedOn, m.Latitude, m.Longitude))
            .ToList();

        var clusters = _clusteringService.Cluster(
            clusterable,
            TimeSpan.FromHours(options.TimeWindowHours),
            options.GeoRadiusKm,
            Math.Max(options.MinPhotosPerChapter, 4));

        if (clusters.Count == 0)
        {
            return new ClusterResult(0, 0, unassigned.Count);
        }

        var category = (await _categoryRepository.FindAsync(c => c.Name == DefaultCategoryName)).FirstOrDefault()
            ?? throw new InvalidOperationException($"The '{DefaultCategoryName}' category is missing from the seeded Categories lookup.");

        var mediaById = unassigned.ToDictionary(m => m.Id);
        var chaptersCreated = 0;
        var mediaAssigned = 0;

        foreach (var clusterIds in clusters)
        {
            var clusterMedia = clusterIds.Select(id => mediaById[id]).ToList();

            var place = await ResolvePlaceAsync(clusterMedia);

            var dates = clusterMedia
                .Select(m => DateOnly.FromDateTime(m.CapturedAt ?? m.UploadedOn))
                .OrderBy(d => d)
                .ToList();

            var startDate = dates.First();
            var endDate = dates.Last();

            // Derived date-range label, not AI-generated — per spec Section 4.
            var title = BuildDateRangeTitle(startDate, endDate);

            // A Confirmed chapter always belongs to a Story elsewhere in this codebase
            // (see ChapterService.ConfirmAsync) — the Stories UI lists Stories, not bare
            // Chapters, so skipping this would create a Chapter no one could ever find.
            var story = new Story
            {
                Id = Guid.NewGuid(),
                PassportId = passport.Id,
                Title = title,
                PlaceId = place?.Id,
                StartDate = startDate,
                EndDate = endDate,
                DisplayOrder = 0,
                CreatedAt = DateTime.UtcNow
            };

            await _storyRepository.AddAsync(story);

            var chapter = new Chapter
            {
                Id = Guid.NewGuid(),
                PassportId = passport.Id,
                StoryId = story.Id,
                CategoryId = category.Id,
                PlaceId = place?.Id,
                Title = title,
                EventDate = startDate,
                DisplayOrder = 0,
                Status = ChapterStatus.Confirmed,
                Source = ChapterSource.AiDetected
            };

            await _chapterRepository.AddAsync(chapter);

            foreach (var media in clusterMedia)
            {
                media.ChapterId = chapter.Id;
            }

            chaptersCreated++;
            mediaAssigned += clusterMedia.Count;
        }

        await _unitOfWork.SaveChangesAsync();

        return new ClusterResult(chaptersCreated, mediaAssigned, unassigned.Count - mediaAssigned);
    }

    private async Task<Place?> ResolvePlaceAsync(List<MediaEntity> clusterMedia)
    {
        var geoTagged = clusterMedia.Where(m => m.Latitude.HasValue && m.Longitude.HasValue).ToList();

        if (geoTagged.Count == 0)
        {
            return null;
        }

        var centroidLat = geoTagged.Average(m => (double)m.Latitude!.Value);
        var centroidLon = geoTagged.Average(m => (double)m.Longitude!.Value);

        var candidates = await _placeRepository.FindAsync(p => p.Latitude != null && p.Longitude != null);

        var nearby = candidates
            .Select(p => new { Place = p, Distance = DistanceKm(centroidLat, centroidLon, (double)p.Latitude!.Value, (double)p.Longitude!.Value) })
            .Where(x => x.Distance <= SamePlaceReuseKm)
            .OrderBy(x => x.Distance)
            .FirstOrDefault();

        if (nearby is not null)
        {
            return nearby.Place;
        }

        // No real reverse-geocoding available — name stays generic until the user edits
        // it, same as TripDetectionService.
        var place = new Place
        {
            Id = Guid.NewGuid(),
            Name = "Unnamed Location",
            Latitude = (decimal)centroidLat,
            Longitude = (decimal)centroidLon
        };

        await _placeRepository.AddAsync(place);
        return place;
    }

    private static string BuildDateRangeTitle(DateOnly start, DateOnly end)
    {
        if (start == end)
        {
            return start.ToString("MMM d");
        }

        return start.Month == end.Month
            ? $"{start:MMM d}–{end:d}"
            : $"{start:MMM d} – {end:MMM d}";
    }

    private static double DistanceKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double earthRadiusKm = 6371;

        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadiusKm * c;
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180;
}
