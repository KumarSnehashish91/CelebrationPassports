namespace CelebrationPassports.Web.Interfaces;

public interface IMediaService
{
    Task<Guid?> UploadAsync(IFormFile file);

    Task<Guid?> UploadToChapterAsync(Guid chapterId, IFormFile file);

    Task<string?> GetUrlAsync(Guid mediaId);
}
