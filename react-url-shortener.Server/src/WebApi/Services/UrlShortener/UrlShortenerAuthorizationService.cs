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

    public async Task<bool> IsUserAuthorizedAsync(Guid userId, Guid urlId, string[] roles)
    {
        bool isOwner = await IsUserOwnsUrlAsync(userId, urlId);
        bool isAdmin = roles.Contains("Admin");
        return isOwner || isAdmin;
    }
}