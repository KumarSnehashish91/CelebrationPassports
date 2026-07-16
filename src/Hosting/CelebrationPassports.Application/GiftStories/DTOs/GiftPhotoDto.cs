namespace CelebrationPassports.Application.GiftStories.DTOs;

public class GiftPhotoDto
{
    public Guid Id { get; set; }

    public string Url { get; set; } = string.Empty;

    public string? UserInsight { get; set; }

    public string? AiGeneratedInsight { get; set; }

    public int DisplayOrder { get; set; }

    public bool HasInsight => !string.IsNullOrWhiteSpace(UserInsight) || !string.IsNullOrWhiteSpace(AiGeneratedInsight);
}
