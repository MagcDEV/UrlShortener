using Microsoft.EntityFrameworkCore;
using UrlShortener.Data;

namespace UrlShortener.Services;

public class UrlShorteningService
{
    public const int NumberOfCharsInShortLink = 7;
    private const string Base62Chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private readonly Random _random = new();
    private readonly AppDbContext _db;

    public UrlShorteningService(AppDbContext appDbContext)
    {
        _db = appDbContext;
    }

    public async Task<string> GenerateShortLink()
    {
        while (true)
        {
            var shortLink = new char[NumberOfCharsInShortLink];
            for (int i = 0; i < NumberOfCharsInShortLink; i++)
            {
                shortLink[i] = Base62Chars[_random.Next(Base62Chars.Length)];
            }

            var code = new string(shortLink);

            if (!await _db.ShortenedUrls.AnyAsync(u => u.Code == code))
            {
                return code;
            }
        }
    }
}