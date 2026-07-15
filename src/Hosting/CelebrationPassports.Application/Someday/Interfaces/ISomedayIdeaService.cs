using CelebrationPassports.Application.Someday.DTOs;

namespace CelebrationPassports.Application.Someday.Interfaces;

public interface ISomedayIdeaService
{
    Task<SomedayIdeaDto> CreateAsync(Guid userId, Guid passportId, CreateSomedayIdeaRequest request);

    Task<IReadOnlyList<SomedayIdeaDto>> GetByPassportAsync(Guid userId, Guid passportId);

    Task<SomedayIdeaDto> UpdateAsync(Guid userId, Guid ideaId, UpdateSomedayIdeaRequest request);

    Task DeleteAsync(Guid userId, Guid ideaId);

    // Creates a Draft Event (this codebase's equivalent of the spec's "Plan") titled
    // after the idea and links it back via ConvertedToEventId — the user finishes
    // scoping it out (type, date, location, etc.) through the normal Events wizard.
    Task<SomedayIdeaDto> ConvertToEventAsync(Guid userId, Guid ideaId);
}
