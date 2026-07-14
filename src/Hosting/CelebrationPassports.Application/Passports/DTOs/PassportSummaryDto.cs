using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Application.Passports.DTOs;

public class PassportSummaryDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public PassportStatus Status { get; set; }

    public int PeopleCount { get; set; }

    public bool IsOwner { get; set; }
}
