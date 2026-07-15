namespace CelebrationPassports.Application.TimeCapsule.DTOs;

public class CreateTimeCapsuleMessageRequest
{
    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public DateTime UnlockDate { get; set; }
}
