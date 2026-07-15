using CelebrationPassports.Web.Models.Stories;

namespace CelebrationPassports.Web.Interfaces;

public interface IStoryNarrativeService
{
    Task<string?> GenerateAsync(StoryDetailViewModel story);
}
