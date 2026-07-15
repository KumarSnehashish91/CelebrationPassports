using CelebrationPassports.Application.Stories.DTOs;

namespace CelebrationPassports.Application.Stories.Interfaces;

public interface IMediaClusteringService
{
    // Returns only the clusters that meet minPhotosPerChapter — media in smaller
    // candidate clusters is simply omitted (left unassigned) rather than returned.
    IReadOnlyList<IReadOnlyList<Guid>> Cluster(
        IReadOnlyList<ClusterableMedia> media,
        TimeSpan timeWindow,
        double geoRadiusKm,
        int minPhotosPerChapter);
}
