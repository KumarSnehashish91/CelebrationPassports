using CelebrationPassports.Web.Models.Recap;

namespace CelebrationPassports.Web.Interfaces;

public interface IRecapService
{
    Task<string?> GenerateAsync(int year, List<RecapMomentViewModel> moments);
}
