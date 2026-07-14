using CelebrationPassports.Infrastructure.Photo.Interfaces;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

namespace CelebrationPassports.Infrastructure.Photo.Services;

public class PhotoMetadataService : IPhotoMetadataService
{
    public PhotoMetadata Extract(Stream content)
    {
        var result = new PhotoMetadata();

        if (!content.CanSeek)
        {
            return result;
        }

        var startPosition = content.Position;

        try
        {
            var directories = ImageMetadataReader.ReadMetadata(content);

            var gps = directories.OfType<GpsDirectory>().FirstOrDefault();
            var location = gps?.GetGeoLocation();

            if (location.HasValue && !location.Value.IsZero)
            {
                result.Latitude = (decimal)location.Value.Latitude;
                result.Longitude = (decimal)location.Value.Longitude;
            }

            var exif = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();

            if (exif is not null && exif.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out var capturedAt))
            {
                result.CapturedAt = DateTime.SpecifyKind(capturedAt, DateTimeKind.Local).ToUniversalTime();
            }
        }
        catch (ImageProcessingException)
        {
            // Not a format MetadataExtractor recognizes, or malformed EXIF — treat as
            // "no metadata" rather than failing the whole upload over it.
        }
        finally
        {
            content.Position = startPosition;
        }

        return result;
    }
}
