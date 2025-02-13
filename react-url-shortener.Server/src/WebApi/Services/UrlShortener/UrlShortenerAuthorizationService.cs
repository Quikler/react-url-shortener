using DAL;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Services.UrlShortener;

public class UrlShortenerAuthorizationService(AppDbContext dbContext) : IUrlShortenerAuthorizationService
{
    public async Task<bool> IsUserOwnsShortenedUrlAsync(Guid userId, string originalUrl)
    {
        return await dbContext.Urls
            .Where(u => u.UrlOriginal == originalUrl)
            .AnyAsync(u => u.UserId == userId);
    }
}