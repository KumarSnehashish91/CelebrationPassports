using CelebrationPassports.Web.Models.Passports;

namespace CelebrationPassports.Web.Interfaces;

public interface IPassportService
{
    Task<List<PassportListItemViewModel>> GetMineAsync();

    Task<bool> CreateAsync(CreatePassportViewModel model);

    Task<int> GetStampCountAsync();
}
