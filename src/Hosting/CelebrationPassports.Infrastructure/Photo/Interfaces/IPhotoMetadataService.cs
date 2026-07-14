namespace CelebrationPassports.Infrastructure.Photo.Interfaces;

public class PhotoMetadata
{
    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public DateTime? CapturedAt { get; set; }
}

public interface IPhotoMetadataService
{
    // Reads EXIF GPS/capture-date tags from the given photo stream. Leaves the stream
    // positioned at 0 on return so the caller can still read it again afterwards (e.g. to
    // save the file) — never assume this consumed the stream. Returns an all-null result
    // (never throws) for non-photo content or photos with no EXIF data, since most of
    // this is genuinely optional/absent on plenty of real photos (screenshots, WhatsApp
    // re-compressed images, GPS turned off, etc.).
    PhotoMetadata Extract(Stream content);
}
