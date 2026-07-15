using CelebrationPassports.Web.Models.MemoryMap;

namespace CelebrationPassports.Web.Interfaces;

public interface IMemoryMapService
{
    Task<List<MemoryMapPinViewModel>> GetByPassportAsync(Guid passportId);
}
