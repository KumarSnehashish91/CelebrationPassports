using CelebrationPassports.Application.Stories.DTOs;
using CelebrationPassports.Application.Stories.Services;
using Xunit;

namespace CelebrationPassports.UnitTests.Stories;

public class MediaClusteringServiceTests
{
    private readonly MediaClusteringService _service = new();

    private static readonly TimeSpan TimeWindow = TimeSpan.FromHours(24);
    private const double GeoRadiusKm = 2;
    private const int MinPhotosPerChapter = 5;

    // Roughly 111km per degree of latitude — used to place points a known distance apart.
    private static decimal LatOffsetForKm(double km) => (decimal)(km / 111.0);

    [Fact]
    public void Cluster_BelowThreshold_ReturnsNoChapters()
    {
        var baseTime = new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc);

        var media = Enumerable.Range(0, 3)
            .Select(i => new ClusterableMedia(Guid.NewGuid(), baseTime.AddMinutes(i), 12.9716m, 77.5946m))
            .ToList();

        var result = _service.Cluster(media, TimeWindow, GeoRadiusKm, MinPhotosPerChapter);

        Assert.Empty(result);
    }

    [Fact]
    public void Cluster_MeetsThreshold_ReturnsOneClusterWithAllMedia()
    {
        var baseTime = new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc);

        var media = Enumerable.Range(0, 5)
            .Select(i => new ClusterableMedia(Guid.NewGuid(), baseTime.AddMinutes(i * 10), 12.9716m, 77.5946m))
            .ToList();

        var result = _service.Cluster(media, TimeWindow, GeoRadiusKm, MinPhotosPerChapter);

        Assert.Single(result);
        Assert.Equal(5, result[0].Count);
        Assert.Equal(media.Select(m => m.Id).OrderBy(id => id), result[0].OrderBy(id => id));
    }

    [Fact]
    public void Cluster_TwoSmallUnrelatedGroups_DoesNotMerge()
    {
        var day1 = new DateTime(2026, 3, 1, 10, 0, 0, DateTimeKind.Utc);
        var day5 = new DateTime(2026, 3, 5, 10, 0, 0, DateTimeKind.Utc);

        var media = new List<ClusterableMedia>
        {
            new(Guid.NewGuid(), day1, 12.9716m, 77.5946m),
            new(Guid.NewGuid(), day1.AddMinutes(5), 12.9716m, 77.5946m),
            new(Guid.NewGuid(), day5, 12.9716m, 77.5946m),
            new(Guid.NewGuid(), day5.AddMinutes(5), 12.9716m, 77.5946m)
        };

        var result = _service.Cluster(media, TimeWindow, GeoRadiusKm, MinPhotosPerChapter);

        Assert.Empty(result);
    }

    [Fact]
    public void Cluster_MissingGeoData_FallsBackToTimeOnly()
    {
        var baseTime = new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc);

        var media = Enumerable.Range(0, 5)
            .Select(i => new ClusterableMedia(Guid.NewGuid(), baseTime.AddMinutes(i * 10), null, null))
            .ToList();

        var result = _service.Cluster(media, TimeWindow, GeoRadiusKm, MinPhotosPerChapter);

        Assert.Single(result);
        Assert.Equal(5, result[0].Count);
    }

    [Fact]
    public void Cluster_OutsideTimeWindow_SplitsIntoSeparateClusters()
    {
        var day1 = new DateTime(2026, 3, 1, 10, 0, 0, DateTimeKind.Utc);
        var day3 = day1.AddHours(48);

        var group1 = Enumerable.Range(0, 3).Select(i => new ClusterableMedia(Guid.NewGuid(), day1.AddMinutes(i), 12.9716m, 77.5946m));
        var group2 = Enumerable.Range(0, 3).Select(i => new ClusterableMedia(Guid.NewGuid(), day3.AddMinutes(i), 12.9716m, 77.5946m));
        var media = group1.Concat(group2).ToList();

        // Threshold 3 so each group alone qualifies — proves they were kept separate
        // rather than merged (which would produce one 6-item cluster instead).
        var result = _service.Cluster(media, TimeWindow, GeoRadiusKm, minPhotosPerChapter: 3);

        Assert.Equal(2, result.Count);
        Assert.All(result, cluster => Assert.Equal(3, cluster.Count));
    }

    [Fact]
    public void Cluster_OutsideGeoRadius_SplitsIntoSeparateClusters()
    {
        var baseTime = new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc);

        var nearLat = 12.9716m;
        var farLat = nearLat + LatOffsetForKm(50); // ~50km away — well outside the 2km radius

        var group1 = Enumerable.Range(0, 3).Select(i => new ClusterableMedia(Guid.NewGuid(), baseTime.AddMinutes(i), nearLat, 77.5946m));
        var group2 = Enumerable.Range(0, 3).Select(i => new ClusterableMedia(Guid.NewGuid(), baseTime.AddMinutes(i + 3), farLat, 77.5946m));
        var media = group1.Concat(group2).ToList();

        var result = _service.Cluster(media, TimeWindow, GeoRadiusKm, minPhotosPerChapter: 3);

        Assert.Equal(2, result.Count);
        Assert.All(result, cluster => Assert.Equal(3, cluster.Count));
    }

    [Fact]
    public void Cluster_WithinGeoRadius_StaysInOneCluster()
    {
        var baseTime = new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc);

        var lat = 12.9716m;
        var nearbyLat = lat + LatOffsetForKm(1); // ~1km away — inside the 2km radius

        var media = new List<ClusterableMedia>
        {
            new(Guid.NewGuid(), baseTime, lat, 77.5946m),
            new(Guid.NewGuid(), baseTime.AddMinutes(1), lat, 77.5946m),
            new(Guid.NewGuid(), baseTime.AddMinutes(2), nearbyLat, 77.5946m),
            new(Guid.NewGuid(), baseTime.AddMinutes(3), nearbyLat, 77.5946m),
            new(Guid.NewGuid(), baseTime.AddMinutes(4), lat, 77.5946m)
        };

        var result = _service.Cluster(media, TimeWindow, GeoRadiusKm, MinPhotosPerChapter);

        Assert.Single(result);
        Assert.Equal(5, result[0].Count);
    }
}
