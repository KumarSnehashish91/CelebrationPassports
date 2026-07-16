using System.IO.Compression;
using System.Text.Json;
using CelebrationPassports.Infrastructure.Imports.Interfaces;

namespace CelebrationPassports.Infrastructure.Imports.Services;

// Feature: Import Existing Memories (feature-backlog-v1.1.md, PRESERVE #1) — Google
// Photos Takeout format only for v1. Each media file in the export is generally
// accompanied by a JSON sidecar in the same folder, named "<file>.json" or, when
// Google's own filename-length truncation kicks in, "<file>.supplemental-metadata.json".
// The sidecar's photoTakenTime/geoData is the authoritative timestamp/location for this
// format — EXIF is deliberately not consulted here (PhotoMetadataService no-ops on the
// non-seekable zip-entry stream anyway, so it would add nothing).
public class GooglePhotosImportParser : IGooglePhotosImportParser
{
    private static readonly HashSet<string> MediaExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".heic", ".webp", ".gif", ".mp4", ".mov", ".webm"
    };

    public int CountMediaEntries(string archiveFilePath)
    {
        using var archive = ZipFile.OpenRead(archiveFilePath);
        return archive.Entries.Count(IsMediaEntry);
    }

    public async Task ProcessArchiveAsync(
        string archiveFilePath,
        Func<ParsedImportMedia, Task> onMediaFound,
        CancellationToken cancellationToken)
    {
        using var archive = ZipFile.OpenRead(archiveFilePath);

        var sidecarLookup = archive.Entries
            .Where(e => e.FullName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            .ToDictionary(e => e.FullName, e => e, StringComparer.OrdinalIgnoreCase);

        foreach (var entry in archive.Entries)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!IsMediaEntry(entry))
            {
                continue;
            }

            var (capturedAt, latitude, longitude) = ReadSidecarMetadata(entry, sidecarLookup);

            await using var stream = entry.Open();

            await onMediaFound(new ParsedImportMedia
            {
                Content = stream,
                FileName = Path.GetFileName(entry.FullName),
                Length = entry.Length,
                CapturedAt = capturedAt,
                Latitude = latitude,
                Longitude = longitude
            });
        }
    }

    private static bool IsMediaEntry(ZipArchiveEntry entry) =>
        !string.IsNullOrEmpty(entry.Name) && MediaExtensions.Contains(Path.GetExtension(entry.FullName));

    private static (DateTime? CapturedAt, decimal? Latitude, decimal? Longitude) ReadSidecarMetadata(
        ZipArchiveEntry mediaEntry, Dictionary<string, ZipArchiveEntry> sidecarLookup)
    {
        var candidateNames = new[]
        {
            $"{mediaEntry.FullName}.json",
            $"{mediaEntry.FullName}.supplemental-metadata.json"
        };

        var sidecar = candidateNames
            .Select(name => sidecarLookup.GetValueOrDefault(name))
            .FirstOrDefault(e => e is not null);

        if (sidecar is null)
        {
            return (null, null, null);
        }

        try
        {
            using var sidecarStream = sidecar.Open();
            using var doc = JsonDocument.Parse(sidecarStream);
            var root = doc.RootElement;

            DateTime? capturedAt = null;

            if (root.TryGetProperty("photoTakenTime", out var takenTime) &&
                takenTime.TryGetProperty("timestamp", out var timestampProp) &&
                long.TryParse(timestampProp.GetString(), out var epochSeconds))
            {
                capturedAt = DateTimeOffset.FromUnixTimeSeconds(epochSeconds).UtcDateTime;
            }

            decimal? latitude = null;
            decimal? longitude = null;

            if (root.TryGetProperty("geoData", out var geo) &&
                geo.TryGetProperty("latitude", out var latProp) &&
                geo.TryGetProperty("longitude", out var lonProp))
            {
                var lat = latProp.GetDecimal();
                var lon = lonProp.GetDecimal();

                // Google sets (0,0) when no location was recorded rather than omitting the
                // fields — treat that as "no geo data", not "null island".
                if (lat != 0 || lon != 0)
                {
                    latitude = lat;
                    longitude = lon;
                }
            }

            return (capturedAt, latitude, longitude);
        }
        catch (JsonException)
        {
            return (null, null, null);
        }
    }
}
