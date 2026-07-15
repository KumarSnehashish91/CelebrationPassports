using CelebrationPassports.Application.Dashboard.DTOs;
using CelebrationPassports.Application.Dashboard.Interfaces;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Enums;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using MediaEntity = CelebrationPassports.Persistence.Entities.Media;

namespace CelebrationPassports.Application.Dashboard.Services;

// Replaces the Dashboard's old hardcoded Memories/Trips/Countries numbers with real
// counts. Deliberately simple aggregate queries (fetch-then-count in memory) rather than
// SQL-side COUNT/DISTINCT — this app's data volumes are small (a household's worth of
// events/chapters), and it keeps this consistent with how the rest of the app queries
// through IGenericRepository rather than hand-rolled SQL.
public class DashboardStatsService : IDashboardStatsService
{
    private readonly IPassportRepository _passportRepository;
    private readonly IGenericRepository<MediaEntity> _mediaRepository;
    private readonly IGenericRepository<Event> _eventRepository;
    private readonly IGenericRepository<Chapter> _chapterRepository;
    private readonly IGenericRepository<Place> _placeRepository;

    public DashboardStatsService(
        IPassportRepository passportRepository,
        IGenericRepository<MediaEntity> mediaRepository,
        IGenericRepository<Event> eventRepository,
        IGenericRepository<Chapter> chapterRepository,
        IGenericRepository<Place> placeRepository)
    {
        _passportRepository = passportRepository;
        _mediaRepository = mediaRepository;
        _eventRepository = eventRepository;
        _chapterRepository = chapterRepository;
        _placeRepository = placeRepository;
    }

    public async Task<DashboardStatsDto> GetSummaryAsync(Guid userId)
    {
        var passports = await _passportRepository.GetForUserAsync(userId);
        var passportIds = passports.Select(p => p.Id).ToHashSet();

        if (passportIds.Count == 0)
        {
            return new DashboardStatsDto();
        }

        var mediaCount = (await _mediaRepository.FindAsync(m =>
            !m.IsDeleted && m.ChapterId != null && passportIds.Contains(m.Chapter!.PassportId)))
            .Count;

        var events = await _eventRepository.FindAsync(e => passportIds.Contains(e.PassportId) && !e.IsDeleted);

        var tripsCount = events.Count(e =>
            e.EventType == EventType.Vacation && e.Status is not (EventStatus.Draft or EventStatus.Cancelled));

        var eventPlaceIds = events.Where(e => e.PlaceId.HasValue).Select(e => e.PlaceId!.Value);

        var chapters = await _chapterRepository.FindAsync(c =>
            passportIds.Contains(c.PassportId) && !c.IsDeleted && c.PlaceId != null);

        var chapterPlaceIds = chapters.Select(c => c.PlaceId!.Value);

        var allPlaceIds = eventPlaceIds.Concat(chapterPlaceIds).ToHashSet();

        var countriesCount = 0;

        if (allPlaceIds.Count > 0)
        {
            var places = await _placeRepository.FindAsync(p => allPlaceIds.Contains(p.Id) && p.Country != null);
            countriesCount = places.Select(p => p.Country!.Trim().ToLowerInvariant()).Distinct().Count();
        }

        return new DashboardStatsDto
        {
            MemoriesCount = mediaCount,
            TripsCount = tripsCount,
            CountriesCount = countriesCount
        };
    }
}
