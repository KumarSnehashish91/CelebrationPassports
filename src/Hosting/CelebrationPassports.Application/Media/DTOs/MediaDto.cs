using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Application.Media.DTOs;

public class MediaDto
{
    public Guid Id { get; set; }

    public Guid? ChapterId { get; set; }

    public string Url { get; set; } = string.Empty;

    public MediaType Type { get; set; }

    public Guid UploadedBy { get; set; }
}
