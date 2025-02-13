using DAL;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Services.UrlShortener;

public class UrlShortenerAuthorizationService(AppDbContext dbContext) : IUrlShortenerAuthorizationService
{
    public async Task<bool> IsUserOwnsUrlAsync(Guid userId, Guid urlId)
    {
        return await dbContext.Urls
            .Where(u => u.Id == urlId)
            .AnyAsync(u => u.UserId == userId);
    }
}