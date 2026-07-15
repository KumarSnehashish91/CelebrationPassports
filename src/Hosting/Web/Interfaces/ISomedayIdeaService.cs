using CelebrationPassports.Web.Models.Ideas;

namespace CelebrationPassports.Web.Interfaces;

public interface ISomedayIdeaService
{
    Task<List<SomedayIdeaViewModel>> GetByPassportAsync(Guid passportId);

    Task<bool> CreateAsync(Guid passportId, string title, string? notes);

    Task<bool> DeleteAsync(Guid id);

    // Returns the newly created Draft Event's id on success, so the caller can send the
    // user straight into the Events wizard to finish scoping it out.
    Task<Guid?> ConvertToEventAsync(Guid id);
}
