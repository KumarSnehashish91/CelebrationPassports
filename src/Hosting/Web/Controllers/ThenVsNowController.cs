using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.MemoryMap;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.Web.Controllers;

// "Then vs. Now" (feature-backlog-v1.1.md, RELIVE #8) — query-only, built entirely on
// top of the existing Memory Map pins (Chapter + Place). Finds chapters that happened
// close together geographically but in different years, and pairs the earliest with the
// most recent for each such place. No new entities, no AI — same "reuse what's already
// real" pattern as the rest of this app.
[Authorize]
public class ThenVsNowController : Controller
{
    private const double ClusterRadiusKm = 1.5;

    private readonly IPassportService _passportService;
    private readonly IMemoryMapService _memoryMapService;

    public ThenVsNowController(IPassportService passportService, IMemoryMapService memoryMapService)
    {
        _passportService = passportService;
        _memoryMapService = memoryMapService;
    }

    public async Task<IActionResult> Index()
    {
        var passports = await _passportService.GetMineAsync();
        var allPins = new List<MemoryMapPinViewModel>();

        foreach (var passport in passports)
        {
            allPins.AddRange(await _memoryMapService.GetByPassportAsync(passport.Id));
        }

        var clusters = ClusterByLocation(allPins, ClusterRadiusKm);

        var pairs = new List<ThenVsNowPairViewModel>();

        foreach (var cluster in clusters)
        {
            if (cluster.Select(p => p.EventDate.Year).Distinct().Count() < 2)
            {
                continue;
            }

            var sorted = cluster.OrderBy(p => p.EventDate).ToList();
            var earliest = sorted.First();
            var latest = sorted.Last();

            pairs.Add(new ThenVsNowPairViewModel
            {
                PlaceLabel = latest.PlaceName ?? earliest.PlaceName ?? "This place",
                Earlier = earliest,
                Later = latest
            });
        }

        return View(pairs.OrderByDescending(p => p.Later.EventDate).ToList());
    }

    // Simple single-linkage clustering — fine at this app's data scale (a household's
    // worth of geo-tagged chapters), no need for a spatial index.
    private static List<List<MemoryMapPinViewModel>> ClusterByLocation(List<MemoryMapPinViewModel> pins, double radiusKm)
    {
        var clusters = new List<List<MemoryMapPinViewModel>>();
        var remaining = new List<MemoryMapPinViewModel>(pins);

        while (remaining.Count > 0)
        {
            var cluster = new List<MemoryMapPinViewModel> { remaining[0] };
            remaining.RemoveAt(0);

            bool addedAny;
            do
            {
                addedAny = false;

                for (var i = remaining.Count - 1; i >= 0; i--)
                {
                    var candidate = remaining[i];

                    if (cluster.Any(p => DistanceKm(p.Latitude, p.Longitude, candidate.Latitude, candidate.Longitude) <= radiusKm))
                    {
                        cluster.Add(candidate);
                        remaining.RemoveAt(i);
                        addedAny = true;
                    }
                }
            } while (addedAny);

            clusters.Add(cluster);
        }

        return clusters;
    }

    private static double DistanceKm(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
    {
        const double earthRadiusKm = 6371.0;

        var dLat = ToRadians((double)(lat2 - lat1));
        var dLon = ToRadians((double)(lon2 - lon1));

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians((double)lat1)) * Math.Cos(ToRadians((double)lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadiusKm * c;
    }

    private static double ToRadians(double deg) => deg * Math.PI / 180;
}
