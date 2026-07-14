using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Application.Passports.DTOs;

public class PassportDetailDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public PassportStatus Status { get; set; }

    public Guid OwnerUserId { get; set; }

    public DateTime CreatedOn { get; set; }

    public List<PassportPersonDto> People { get; set; } = new();
}
