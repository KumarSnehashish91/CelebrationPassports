namespace CelebrationPassports.Application.Media.DTOs;

// Decouples the Application layer from ASP.NET Core's IFormFile — the controller
// converts IFormFile into this before calling into the service.
public class FileUploadRequest
{
    public required Stream Content { get; set; }

    public required string FileName { get; set; }

    public long Length { get; set; }
}
