using System.Security.Cryptography;
using System.Text;
using CelebrationPassports.Application.Calendar.Configuration;
using CelebrationPassports.Application.Calendar.Interfaces;
using Microsoft.Extensions.Options;

namespace CelebrationPassports.Application.Calendar.Services;

public class CalendarFeedTokenService : ICalendarFeedTokenService
{
    private readonly IOptionsMonitor<CalendarFeedOptions> _options;

    public CalendarFeedTokenService(IOptionsMonitor<CalendarFeedOptions> options)
    {
        _options = options;
    }

    public string GenerateToken(Guid userId)
    {
        var key = Encoding.UTF8.GetBytes(_options.CurrentValue.SigningKey);
        var message = Encoding.UTF8.GetBytes(userId.ToString("N"));
        var hash = HMACSHA256.HashData(key, message);

        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    public bool ValidateToken(Guid userId, string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return false;
        }

        var expected = Encoding.UTF8.GetBytes(GenerateToken(userId));
        var actual = Encoding.UTF8.GetBytes(token);

        // Fixed-time comparison — a timing attack could otherwise leak the token one
        // byte at a time.
        return actual.Length == expected.Length && CryptographicOperations.FixedTimeEquals(expected, actual);
    }
}
