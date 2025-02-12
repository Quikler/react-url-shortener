using System.Security.Cryptography;
using System.Text;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using WebApi.Common;
using WebApi.DTOs;

namespace WebApi.Services.UrlShortener;

public class UrlShortenerService(AppDbContext dbContext) : IUrlShortenerService
{
    public async Task<Result<string, FailureDto>> GetOriginalUrlAsync(string shortenedUrl)
    {
        var url = await dbContext.Urls
            .Where(u => u.ShortCode == shortenedUrl)
            .Select(u => u.UrlOriginal)
            .FirstOrDefaultAsync();

        if (url is null) return FailureDto.NotFound("Url not found.");

        return url;
    }

    public async Task<Result<string, FailureDto>> CreateShortenUrlAsync(string originalUrl)
    {
        if (await dbContext.Urls.AnyAsync(u => u.UrlOriginal == originalUrl)) return FailureDto.Conflict("Url already exist.");

        var shortCode = GenerateShortCode();

        var url = new UrlEntity
        {
            UrlOriginal = NormalizeUrl(originalUrl),
            ShortCode = shortCode,
        };

        await dbContext.Urls.AddAsync(url);
        int rows = await dbContext.SaveChangesAsync();

        return rows == 0 ? FailureDto.BadRequest("Cannot create url.") : shortCode;
    }

    public async Task<Result<bool, FailureDto>> DeleteUrlAsync(string shortenedUrl)
    {
        int rows = await dbContext.Urls
            .Where(u => u.ShortCode == shortenedUrl)
            .ExecuteDeleteAsync();

        return rows == 0 ? FailureDto.BadRequest("Cannot delete url.") : true;
    }

    private const string Base62Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    private static string GenerateShortCode(int length = 6)
    {
        var randomBytes = RandomNumberGenerator.GetBytes(length);
        var shortCode = new StringBuilder(length);

        foreach (var b in randomBytes)
        {
            shortCode.Append(Base62Chars[b % Base62Chars.Length]);
        }

        return shortCode.ToString();
    }

    private static string NormalizeUrl(string input)
    {
        if (!input.StartsWith("http://") && !input.StartsWith("https://"))
        {
            input = "https://" + input;
        }
        return input;
    }
}