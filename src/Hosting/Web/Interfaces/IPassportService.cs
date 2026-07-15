using CelebrationPassports.Web.Models.Passports;

namespace CelebrationPassports.Web.Interfaces;

public interface IPassportService
{
    Task<List<PassportListItemViewModel>> GetMineAsync();

    Task<PassportDetailViewModel?> GetByIdAsync(Guid id);

    Task<bool> CreateAsync(CreatePassportViewModel model);

    Task<int> GetStampCountAsync();
}
