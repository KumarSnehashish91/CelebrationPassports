using System.Security.Cryptography;
using System.Text;
using CelebrationPassports.Application.Guestbook.Configuration;
using CelebrationPassports.Application.Guestbook.Interfaces;
using Microsoft.Extensions.Options;

namespace CelebrationPassports.Application.Guestbook.Services;

public class GuestbookTokenService : IGuestbookTokenService
{
    private readonly IOptionsMonitor<GuestbookOptions> _options;

    public GuestbookTokenService(IOptionsMonitor<GuestbookOptions> options)
    {
        _options = options;
    }

    public string GenerateToken(Guid chapterId)
    {
        var key = Encoding.UTF8.GetBytes(_options.CurrentValue.SigningKey);
        var message = Encoding.UTF8.GetBytes(chapterId.ToString("N"));
        var hash = HMACSHA256.HashData(key, message);

        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    public bool ValidateToken(Guid chapterId, string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return false;
        }

        var expected = Encoding.UTF8.GetBytes(GenerateToken(chapterId));
        var actual = Encoding.UTF8.GetBytes(token);

        return actual.Length == expected.Length && CryptographicOperations.FixedTimeEquals(expected, actual);
    }
}
