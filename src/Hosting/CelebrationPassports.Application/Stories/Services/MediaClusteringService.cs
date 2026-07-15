using CelebrationPassports.Application.Stories.DTOs;
using CelebrationPassports.Application.Stories.Interfaces;

namespace CelebrationPassports.Application.Stories.Services;

// Pure, deterministic clustering — no EF/DB dependency, so it's unit-testable with
// plain in-memory data (per auto-chapter-detection.md, Section 6). Walks media sorted
// by timestamp and greedily extends the current cluster while each next item stays
// within TIME_WINDOW and GEO_RADIUS of the *last* item added to it; a gap in either
// dimension starts a new cluster. Missing coordinates on either side falls back to
// time-only clustering rather than disqualifying the pair, per spec Section 4.
public class MediaClusteringService : IMediaClusteringService
{
    public IReadOnlyList<IReadOnlyList<Guid>> Cluster(
        IReadOnlyList<ClusterableMedia> media,
        TimeSpan timeWindow,
        double geoRadiusKm,
        int minPhotosPerChapter)
    {
        var sorted = media.OrderBy(m => m.Timestamp).ToList();
        var clusters = new List<List<ClusterableMedia>>();

        foreach (var item in sorted)
        {
            var current = clusters.Count > 0 ? clusters[^1] : null;

            if (current is not null && IsWithinCluster(current[^1], item, timeWindow, geoRadiusKm))
            {
                current.Add(item);
            }
            else
            {
                clusters.Add([item]);
            }
        }

        return clusters
            .Where(c => c.Count >= minPhotosPerChapter)
            .Select(c => (IReadOnlyList<Guid>)c.Select(m => m.Id).ToList())
            .ToList();
    }

    private static bool IsWithinCluster(ClusterableMedia last, ClusterableMedia candidate, TimeSpan timeWindow, double geoRadiusKm)
    {
        if (candidate.Timestamp - last.Timestamp > timeWindow)
        {
            return false;
        }

        if (last.Latitude.HasValue && last.Longitude.HasValue && candidate.Latitude.HasValue && candidate.Longitude.HasValue)
        {
            var distance = DistanceKm(
                (double)last.Latitude.Value, (double)last.Longitude.Value,
                (double)candidate.Latitude.Value, (double)candidate.Longitude.Value);

            return distance <= geoRadiusKm;
        }

        return true;
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
