using CelebrationPassports.Application.Media.DTOs;

namespace CelebrationPassports.Application.Media.Interfaces;

public interface IMediaService
{
    Task<MediaDto> UploadAsync(Guid userId, Guid chapterId, FileUploadRequest file);

    Task<IReadOnlyList<MediaDto>> ListByChapterAsync(Guid userId, Guid chapterId);
}
