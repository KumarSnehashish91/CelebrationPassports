using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Persistence.Entities;

public class MediaVariant
{
    public Guid Id { get; set; }

    public Guid MediaId { get; set; }

    public MediaVariantType VariantType { get; set; }

    public string Url { get; set; } = string.Empty;

    public int Width { get; set; }

    public int Height { get; set; }

    public int FileSize { get; set; }

    public virtual Media Media { get; set; } = null!;
}
