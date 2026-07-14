using CelebrationPassports.Application.Notifications.Interfaces;
using CelebrationPassports.Application.Stories.Interfaces;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Enums;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using MediaEntity = CelebrationPassports.Persistence.Entities.Media;

namespace CelebrationPassports.Application.Stories.Services;

// Deliberately simple/heuristic where real AI (photo curation, collage generation,
// reverse geocoding a coordinate into a real place name) would otherwise be needed —
// there's no image-processing or geocoding infrastructure in this project yet, so this
// does the honest, real part (GPS-based distance-from-home detection, off the actual
// EXIF data on the uploaded photos) and leaves place naming to the user at review time,
// same pattern as the rest of this app's "no fake AI" conventions (see e.g. the
// LocalFileStorageService placeholder, or the Events wizard skipping live Maps).
public class TripDetectionService : ITripDetectionService
{
    // "Away from home" threshold — comfortably beyond a normal day-trip commute radius.
    private const double AwayFromHomeKm = 100;

    // Treat photos taken within this radius of each other as "the same place" when
    // deciding whether to reuse an existing Place instead of creating a new one.
    private const double SamePlaceKm = 5;

    private const string TravelCategoryName = "Travel";

    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IGenericRepository<Place> _placeRepository;
    private readonly IGenericRepository<MediaEntity> _mediaRepository;
    private readonly IGenericRepository<Category> _categoryRepository;
    private readonly IPassportRepository _passportRepository;
    private readonly IChapterRepository _chapterRepository;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;

    public TripDetectionService(
        IUserProfileRepository userProfileRepository,
        IGenericRepository<Place> placeRepository,
        IGenericRepository<MediaEntity> mediaRepository,
        IGenericRepository<Category> categoryRepository,
        IPassportRepository passportRepository,
        IChapterRepository chapterRepository,
        INotificationService notificationService,
        IUnitOfWork unitOfWork)
    {
        _userProfileRepository = userProfileRepository;
        _placeRepository = placeRepository;
        _mediaRepository = mediaRepository;
        _categoryRepository = categoryRepository;
        _passportRepository = passportRepository;
        _chapterRepository = chapterRepository;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid?> DetectAsync(Guid userId, IReadOnlyList<Guid> mediaIds)
    {
        if (mediaIds.Count == 0)
        {
            return null;
        }

        var profile = await _userProfileRepository.GetUserProfileByIdAsync(userId);

        if (profile?.HomePlaceId is null)
        {
            return null;
        }

        var home = await _placeRepository.GetByIdAsync(profile.HomePlaceId.Value);

        if (home?.Latitude is null || home.Longitude is null)
        {
            return null;
        }

        var batch = await _mediaRepository.FindAsync(m =>
            mediaIds.Contains(m.Id) && m.UploadedBy == userId && !m.IsDeleted);

        var geoTagged = batch.Where(m => m.Latitude.HasValue && m.Longitude.HasValue).ToList();

        if (geoTagged.Count == 0)
        {
            return null;
        }

        var distances = geoTagged
            .Select(m => DistanceKm((double)home.Latitude!.Value, (double)home.Longitude!.Value, (double)m.Latitude!.Value, (double)m.Longitude!.Value))
            .ToList();

        if (distances.Average() < AwayFromHomeKm)
        {
            return null;
        }

        var passports = await _passportRepository.GetForUserAsync(userId);
        var passport = passports.FirstOrDefault();

        if (passport is null)
        {
            return null;
        }

        var centroidLat = geoTagged.Average(m => (double)m.Latitude!.Value);
        var centroidLon = geoTagged.Average(m => (double)m.Longitude!.Value);

        var place = await FindOrCreateNearbyPlaceAsync(centroidLat, centroidLon);

        var travelCategory = (await _categoryRepository.FindAsync(c => c.Name == TravelCategoryName)).FirstOrDefault();

        if (travelCategory is null)
        {
            // Should never happen — Travel is one of the fixed seeded categories — but a
            // clear failure here beats silently violating the Category FK below.
            throw new InvalidOperationException("The 'Travel' category is missing from the seeded Categories lookup.");
        }

        var eventDate = geoTagged
            .Where(m => m.CapturedAt.HasValue)
            .Select(m => DateOnly.FromDateTime(m.CapturedAt!.Value))
            .OrderBy(d => d)
            .FirstOrDefault();

        if (eventDate == default)
        {
            eventDate = DateOnly.FromDateTime(DateTime.UtcNow);
        }

        var chapter = new Chapter
        {
            Id = Guid.NewGuid(),
            PassportId = passport.Id,
            StoryId = null,
            CategoryId = travelCategory.Id,
            PlaceId = place?.Id,
            Title = $"New Trip - {eventDate:MMMM yyyy}",
            EventDate = eventDate,
            DisplayOrder = 0,
            Status = ChapterStatus.Draft,
            Source = ChapterSource.AiDetected
        };

        await _chapterRepository.AddAsync(chapter);

        foreach (var media in batch)
        {
            media.ChapterId = chapter.Id;
        }

        await _unitOfWork.SaveChangesAsync();

        await _notificationService.CreateAsync(
            userId,
            NotificationType.Memory,
            "We spotted a trip!",
            $"Looks like you were away from home — we've drafted a new chapter with {batch.Count} photo(s) for you to review.",
            ReferenceType.Chapter,
            chapter.Id,
            $"/Stories/ReviewChapter/{chapter.Id}");

        return chapter.Id;
    }

    private async Task<Place?> FindOrCreateNearbyPlaceAsync(double lat, double lon)
    {
        var candidates = await _placeRepository.FindAsync(p => p.Latitude != null && p.Longitude != null);

        var nearby = candidates
            .Select(p => new { Place = p, Distance = DistanceKm(lat, lon, (double)p.Latitude!.Value, (double)p.Longitude!.Value) })
            .Where(x => x.Distance <= SamePlaceKm)
            .OrderBy(x => x.Distance)
            .FirstOrDefault();

        if (nearby is not null)
        {
            return nearby.Place;
        }

        // No real reverse-geocoding available — name stays generic until the user edits
        // it during review (see the scope note at the top of this file).
        var place = new Place
        {
            Id = Guid.NewGuid(),
            Name = "Unnamed Location",
            Latitude = (decimal)lat,
            Longitude = (decimal)lon
        };

        await _placeRepository.AddAsync(place);
        return place;
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
