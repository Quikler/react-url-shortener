using System.Security.Cryptography;
using System.Text;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using WebApi.Common;
using WebApi.DTOs;
using WebApi.DTOs.Url;
using WebApi.Mapping;

namespace WebApi.Services.UrlShortener;

public class UrlShortenerService(AppDbContext dbContext, IUrlShortenerAuthorizationService urlShortenerAuthorizationService) : IUrlShortenerService
{
    public async Task<Result<List<UrlDto>, FailureDto>> GetAllAsync()
    {
        var urls = await dbContext.Urls.ToListAsync();

        return urls
            .Select(u => u.ToUrlDto())
            .ToList();
    }

    public async Task<Result<string, FailureDto>> GetOriginalUrlByShortCodeAsync(string shortCode)
    {
        var urlOriginal = await dbContext.Urls
            .Where(u => u.ShortCode == shortCode)
            .Select(u => u.UrlOriginal) // Optimize SQL query a bit
            .FirstOrDefaultAsync();

        return urlOriginal is null ? FailureDto.NotFound("Url not found.") : urlOriginal;
    }

    public async Task<Result<UrlDto, FailureDto>> CreateShortenUrlAsync(string originalUrl, Guid userId)
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

        return rows == 0 ? FailureDto.BadRequest("Cannot create url.") : url.ToUrlDto();
    }

    public async Task<Result<bool, FailureDto>> DeleteUrlAsync(Guid urlId, Guid userId, string[] userRoles)
    {
        if (!await dbContext.Urls.AnyAsync(u => u.Id == urlId))
        {
            return FailureDto.NotFound("Url not found.");
        }

        if (!await urlShortenerAuthorizationService.IsUserAuthorizedAsync(userId, urlId, userRoles))
        {
            return FailureDto.Forbidden("User doesn't authorized to url.");
        }

        int rows = await dbContext.Urls
            .Where(u => u.Id == urlId)
            .ExecuteDeleteAsync();

        return rows == 0 ? FailureDto.BadRequest("Cannot delete url.") : true;
    }

    public async Task<Result<UrlInfoDto, FailureDto>> GetInfoAsync(Guid userId, Guid urlId, string[] userRoles)
    {
        if (!await urlShortenerAuthorizationService.IsUserAuthorizedAsync(userId, urlId, userRoles))
        {
            return FailureDto.Forbidden("User doesn't authorized to url.");
        }

        var url = await dbContext.Urls
            .Where(u => u.Id == urlId)
            .Include(u => u.User)
            .Select(u => u.ToUrlInfoDto())
            .FirstOrDefaultAsync();

        return url is null ? FailureDto.NotFound("Url not found.") : url;
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