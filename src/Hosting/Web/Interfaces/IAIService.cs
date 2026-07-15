namespace CelebrationPassports.Web.Interfaces;

public interface IAIService
{
    Task<string?> GenerateAsync(string prompt);
}
