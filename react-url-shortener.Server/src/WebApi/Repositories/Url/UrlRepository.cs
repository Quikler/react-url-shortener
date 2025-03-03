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

    public async Task<int> DeleteUrlAsync(Guid urlId)
    {
        int rows = await _dbContext.Urls
            .Where(u => u.Id == urlId)
            .ExecuteDeleteAsync();

        if (rows > 0)
        {
            _memoryCache.Remove(CacheKeys.Urls);
            _memoryCache.Remove($"{CacheKeys.UrlId}-{urlId}");
        }

        return rows;
    }

    public async Task<PaginationDto<UrlDto>> GetAllUrlDtoAsync(int pageNumber, int pageSize)
    {
        PaginationDto<UrlDto>? urls = await _memoryCache.GetOrCreateAsync(CacheKeys.Urls, async entry =>
        {
            entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

            var count = await _dbContext.Urls.CountAsync();
            var urls = await _dbContext.Urls
                .AsNoTracking()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(UrlEntityProjection.UrlDto)
                .ToListAsync();

            return urls.ToPagination(u => u, count, pageNumber, pageSize);
        });

        return urls ?? PaginationDto<UrlDto>.Empty;
    }

    public async Task<UrlInfoDto?> GetUrlInfoDtoAsync(Guid urlId)
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

    public Task<string?> GetOriginalUrlByShortCodeAsync(string shortCode)
    {
        return _dbContext.Urls
            .AsNoTracking()
            .Where(u => u.ShortCode == shortCode)
            .Select(u => u.UrlOriginal)
            .FirstOrDefaultAsync();
    }

    public Task<bool> IsUrlOriginalExistAsync(string originalUrl)
    {
        return _dbContext.Urls.AnyAsync(u => u.UrlOriginal == originalUrl);
    }

    public Task<bool> IsUrlByIdExistAsync(Guid urlId)
    {
        return _dbContext.Urls.AnyAsync(u => u.Id == urlId);
    }

    public async Task AddUrlAsync(UrlEntity urlEntity)
    {
        await _dbContext.Urls.AddAsync(urlEntity);
        _memoryCache.Remove(CacheKeys.Urls);
    }

    public void AddUrl(UrlEntity urlEntity)
    {
        _dbContext.Urls.Add(urlEntity);
        _memoryCache.Remove(CacheKeys.Urls);
    }

    public Task<bool> IsUserOwnsUrlAsync(Guid userId, Guid urlId)
    {
        return _dbContext.Urls.AnyAsync(u => u.Id == urlId && u.UserId == userId);
    }

    public async Task<bool> IsUserOwnerOrAdminAsync(Guid userId, Guid urlId, string[] roles)
    {
        return roles.Contains("Admin") || await IsUserOwnsUrlAsync(userId, urlId);
    }
}