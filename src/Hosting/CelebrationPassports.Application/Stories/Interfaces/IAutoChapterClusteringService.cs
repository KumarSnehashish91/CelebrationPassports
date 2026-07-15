using CelebrationPassports.Application.Stories.DTOs;

namespace CelebrationPassports.Application.Stories.Interfaces;

public interface IAutoChapterClusteringService
{
    // Media has no direct PassportId (only Chapter does) — unassigned media is scoped
    // by uploader instead, same as TripDetectionService, and lands in that user's first
    // Passport when a cluster becomes a Chapter.
    Task<ClusterResult> ClusterUnassignedMediaForUserAsync(Guid userId);
}
