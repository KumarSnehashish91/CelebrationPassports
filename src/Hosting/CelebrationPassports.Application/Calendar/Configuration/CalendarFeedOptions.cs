namespace CelebrationPassports.Application.Calendar.Configuration;

public class CalendarFeedOptions
{
    // HMAC key used to derive a per-user feed token — deliberately separate from Jwt:Key
    // so rotating one doesn't invalidate the other; calendar tokens are long-lived
    // (subscribed calendar apps poll the same URL indefinitely) while JWTs are short-lived.
    public string SigningKey { get; set; } = string.Empty;
}
