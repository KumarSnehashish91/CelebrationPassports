namespace CelebrationPassports.Infrastructure.AI.Models;

public class AIRequest
{
    public string Model { get; set; } = string.Empty;

    public string Prompt { get; set; } = string.Empty;

    public bool Stream { get; set; }
}