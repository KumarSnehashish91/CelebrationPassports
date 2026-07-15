using CelebrationPassports.Application.Media.DTOs;

namespace CelebrationPassports.Application.Media.Interfaces;

public interface IMediaService
{
    // chapterId is optional — a null chapter means an unattached upload ("upload first,
    // sort later"), used e.g. for an Event's cover photo before the event's own guard
    // context applies. pendingClustering marks an unattached upload as a candidate for
    // the auto-chapter background sweep (see AutoChapterClusteringService) — false for
    // uploads like an Event cover photo that should never be swept into a chapter.
    Task<MediaDto> UploadAsync(Guid userId, Guid? chapterId, FileUploadRequest file, bool pendingClustering = false);

    Task<IReadOnlyList<MediaDto>> ListByChapterAsync(Guid userId, Guid chapterId);

    // No passport/chapter access-guard here deliberately — same reasoning as the
    // unattached upload: an Event's cover photo has no chapter context to guard against,
    // and Media rows are only ever reachable by id (no enumeration endpoint), so this is
    // effectively "if you already know the id, you can resolve its URL".
    Task<MediaDto?> GetByIdAsync(Guid id);
}
