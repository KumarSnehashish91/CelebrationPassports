namespace CelebrationPassports.Application.Guestbook.DTOs;

public class SubmitGuestbookEntryRequest
{
    public string GuestName { get; set; } = string.Empty;

    public string? Message { get; set; }
}
