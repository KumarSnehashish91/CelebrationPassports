using CelebrationPassports.Application.Passports.DTOs;

namespace CelebrationPassports.Application.Passports.Interfaces;

public interface IPassportService
{
    Task<PassportDetailDto> CreateAsync(Guid userId, CreatePassportRequest request);

    Task<IReadOnlyList<PassportSummaryDto>> GetMineAsync(Guid userId);

    Task<PassportDetailDto> GetByIdAsync(Guid userId, Guid passportId);
}
