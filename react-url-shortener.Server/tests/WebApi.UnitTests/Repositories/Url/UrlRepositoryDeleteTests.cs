using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Memory;
using Shouldly;
using WebApi.Repositories.Url;
using WebApi.Services.Caching;

namespace WebApi.UnitTests.Repositories.Url;

[Collection("SharedTestUrlCollection")]
public class UrlRepositoryDeleteTests(BaseUrlRepositoryTests baseUrlRepositoryTests) : IAsyncLifetime
{
    private readonly AppDbContext _dbContext = baseUrlRepositoryTests.DbContext;
    private readonly UrlRepository _urlRepository = baseUrlRepositoryTests.UrlRepository;
    private readonly IMemoryCache _memoryCache = baseUrlRepositoryTests.MemoryCache;

    private IDbContextTransaction _dbTransaction = default!;

    [Fact]
    public async Task DeleteUrlAsync_ShouldReturnZero_WhenUrlNotFound()
    {
        // Arrange
        var urlId = Guid.NewGuid();

        // Act
        var rows = await _urlRepository.DeleteUrlAsync(urlId);

        // Assert
        rows.ShouldBe(0);
    }

    [Fact]
    public async Task DeleteUrlAsync_ShouldReturnOneAndRemovesCache_WhenUrlDeleted()
    {
        // Arrange
        var user = new UserEntity
        {
            UserName = "test"
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var urlId = Guid.NewGuid();
        var url = new UrlEntity
        {
            Id = urlId,
            ShortCode = "123456",
            UrlOriginal = "https://example.com",
            UserId = user.Id,
            User = user,
        };

        var cacheKey = $"{CacheKeys.UrlId}-{url.Id}";
        _memoryCache.Set(cacheKey, url);

        _dbContext.Urls.Add(url);
        await _dbContext.SaveChangesAsync();

        // Act
        var rows = await _urlRepository.DeleteUrlAsync(urlId);

        // Assert
        rows.ShouldBe(1);
        _memoryCache.TryGetValue(cacheKey, out _).ShouldBeFalse();
    }

    public async Task InitializeAsync()
    {
        _dbTransaction = await _dbContext.Database.BeginTransactionAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbTransaction.RollbackAsync();
        await _dbTransaction.DisposeAsync();
    }
}