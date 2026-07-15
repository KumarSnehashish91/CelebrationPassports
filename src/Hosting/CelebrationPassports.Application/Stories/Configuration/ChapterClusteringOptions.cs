namespace CelebrationPassports.Application.Stories.Configuration;

public class ChapterClusteringOptions
{
    public double TimeWindowHours { get; set; } = 24;

    public double GeoRadiusKm { get; set; } = 2;

    public int MinPhotosPerChapter { get; set; } = 5;

    public int DebounceMinutes { get; set; } = 2;
}
