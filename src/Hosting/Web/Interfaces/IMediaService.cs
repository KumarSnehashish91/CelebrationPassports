namespace CelebrationPassports.Web.Interfaces;

public interface IMediaService
{
    // pendingClustering=true marks this as a Stories/QuickUpload-style batch awaiting
    // the auto-chapter background sweep — leave false for standalone uploads (e.g. an
    // Event's cover photo) that should never be swept into a chapter.
    Task<Guid?> UploadAsync(IFormFile file, bool pendingClustering = false);

    Task<Guid?> UploadToChapterAsync(Guid chapterId, IFormFile file);

    Task<string?> GetUrlAsync(Guid mediaId);
}
