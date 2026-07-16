namespace CelebrationPassports.Application.Guestbook.Configuration;

public class GuestbookOptions
{
    // Deliberately separate from CalendarFeedOptions.SigningKey and Jwt:Key — rotating
    // one shouldn't invalidate the others.
    public string SigningKey { get; set; } = string.Empty;
}
