namespace CelebrationPassports.Web.Models.MemoryMap;

public class ThenVsNowPairViewModel
{
    public string PlaceLabel { get; set; } = string.Empty;

    public MemoryMapPinViewModel Earlier { get; set; } = null!;

    public MemoryMapPinViewModel Later { get; set; } = null!;

    public int YearsApart => Later.EventDate.Year - Earlier.EventDate.Year;
}
