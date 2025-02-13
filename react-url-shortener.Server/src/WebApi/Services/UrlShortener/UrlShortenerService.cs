using System.Security.Cryptography;
using System.Text;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using WebApi.Common;
using WebApi.DTOs;
using WebApi.Utils.Extensions;

namespace WebApi.Services.UrlShortener;

public class UrlShortenerService(AppDbContext dbContext, IUrlShortenerAuthorizationService urlShortenerAuthorizationService) : IUrlShortenerService
{
    public async Task<Result<string, FailureDto>> GetOriginalUrlAsync(string shortenedUrl)
    {
        var url = await dbContext.Urls
            .Where(u => u.ShortCode == shortenedUrl)
            .Select(u => u.UrlOriginal)
            .FirstOrDefaultAsync();

        return url is null ? FailureDto.NotFound("Url not found.") : url;
    }

    public async Task<Result<string, FailureDto>> CreateShortenUrlAsync(string originalUrl, Guid userId)
    {
        if (await dbContext.Urls.AnyAsync(u => u.UrlOriginal == originalUrl))
        {
            return FailureDto.Conflict("Url already exist.");
        }

        var shortCode = GenerateShortCode();

        var url = new UrlEntity
        {
            UrlOriginal = originalUrl,
            ShortCode = shortCode,
            UserId = userId,
        };

        await dbContext.Urls.AddAsync(url);
        int rows = await dbContext.SaveChangesAsync();

        return rows == 0 ? FailureDto.BadRequest("Cannot create url.") : shortCode;
    }

    public async Task<Result<bool, FailureDto>> DeleteUrlAsync(string shortenedUrl, Guid userId)
    {
        if (!await urlShortenerAuthorizationService.IsUserOwnsShortenedUrlAsync(userId, shortenedUrl))
        {
            return FailureDto.Forbidden("User doesn't own url.");
        }

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
}