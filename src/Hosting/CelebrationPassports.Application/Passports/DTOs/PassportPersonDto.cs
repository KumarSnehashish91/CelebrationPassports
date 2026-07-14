using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Application.Passports.DTOs;

public class PassportPersonDto
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public PassportPersonRole Role { get; set; }
}
