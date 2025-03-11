using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WebApi.DTOs;
using WebApi.DTOs.Url;
using WebApi.Mapping;
using WebApi.Projections.Url;
using WebApi.Services.Caching;

namespace WebApi.Repositories.Url;

public class UrlRepository(AppDbContext dbContext, IMemoryCache memoryCache) : IUrlRepository
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly IMemoryCache _memoryCache = memoryCache;

    public virtual async Task<int> DeleteUrlAsync(Guid urlId)
    {
        int rows = await _dbContext.Urls
            .Where(u => u.Id == urlId)
            .ExecuteDeleteAsync();

        if (rows > 0)
        {
            _memoryCache.Remove($"{CacheKeys.UrlId}-{urlId}");
        }

        return rows;
    }

    public virtual async Task<PaginationDto<UrlDto>> GetAllUrlDtoAsync(int pageNumber, int pageSize)
    {
        pageNumber = Math.Max(1, pageNumber);
        pageSize = Math.Max(1, pageSize);

        var query = _dbContext.Urls.AsNoTracking();

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        var urls = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(UrlEntityProjection.UrlDto)
            .ToListAsync();

        return urls.ToPagination(u => u, totalCount, totalPages, pageNumber, pageSize);
    }

    public virtual async Task<UrlInfoDto?> GetUrlInfoDtoAsync(Guid urlId)
    {
        var url = await _memoryCache.GetOrCreateAsync($"{CacheKeys.UrlId}-{urlId}", entry =>
        {
            entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
            return _dbContext.Urls
                .AsNoTracking()
                .Where(u => u.Id == urlId)
                .Select(UrlEntityProjection.UrlInfoDto)
                .FirstOrDefaultAsync();
        });

        return url;
    }

    public virtual Task<string?> GetOriginalUrlByShortCodeAsync(string shortCode)
    {
        return _dbContext.Urls
            .AsNoTracking()
            .Where(u => u.ShortCode == shortCode)
            .Select(u => u.UrlOriginal)
            .FirstOrDefaultAsync();
    }

    public virtual Task<bool> IsUrlOriginalExistAsync(string originalUrl)
    {
        return _dbContext.Urls.AnyAsync(u => u.UrlOriginal == originalUrl);
    }

    public virtual Task<bool> IsUrlByIdExistAsync(Guid urlId)
    {
        return _dbContext.Urls.AnyAsync(u => u.Id == urlId);
    }

    public virtual void AddUrl(UrlEntity urlEntity)
    {
        _dbContext.Urls.Add(urlEntity);
    }

    public virtual Task<bool> IsUserOwnsUrlAsync(Guid userId, Guid urlId)
    {
        return _dbContext.Urls.AnyAsync(u => u.Id == urlId && u.UserId == userId);
    }

    public virtual async Task<bool> IsUserOwnerOrAdminAsync(Guid userId, Guid urlId, string[] roles)
    {
        var exist = await IsUrlByIdExistAsync(urlId);
        if (!exist)
        {
            return false;
        }

        return roles.Contains("Admin") || await IsUserOwnsUrlAsync(userId, urlId);
    }
}