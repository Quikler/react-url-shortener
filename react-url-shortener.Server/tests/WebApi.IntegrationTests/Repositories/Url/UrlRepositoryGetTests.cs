using AutoFixture;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Memory;
using Shouldly;
using WebApi.Repositories.Url;
using WebApi.Services.Caching;

namespace WebApi.IntegrationTests.Repositories.Url;

[Collection("SharedTestUrlCollection")]
public class UrlRepositoryGetTests : IAsyncLifetime
{
    private AppDbContext DbContext { get; }
    private UrlRepository UrlRepository { get; }
    private Fixture Fixture { get; }
    private IMemoryCache MemoryCache { get; }

    private IDbContextTransaction _dbTransaction = default!;

    private readonly UserEntity _userEntity;
    private readonly List<UrlEntity> _urlEntities;

    public UrlRepositoryGetTests(BaseUrlRepositoryTests baseUrlRepositoryTests)
    {
        DbContext = baseUrlRepositoryTests.DbContext;
        UrlRepository = baseUrlRepositoryTests.UrlRepository;
        Fixture = baseUrlRepositoryTests.Fixture;
        MemoryCache = baseUrlRepositoryTests.MemoryCache;

        _userEntity = Fixture.Create<UserEntity>();

        _urlEntities = [.. Fixture.Build<UrlEntity>()
            .With(u => u.User, _userEntity)
            .With(u => u.UserId, _userEntity.Id)
            .CreateMany(5)];

        _userEntity.Urls = _urlEntities;
    }

    [Fact]
    public async Task GetAllUrlDtoAsync_RetursEmptyPagination_WhenUrlsIsEmpty()
    {
        // Arrange
        int pageNumber = 1, pageSize = 1;

        // Act
        var pagination = await UrlRepository.GetAllUrlDtoAsync(pageNumber, pageSize);

        // Assert
        pagination.Items.ShouldBeEmpty();
        pagination.PageNumber.ShouldBe(pageNumber);
        pagination.PageSize.ShouldBe(pageSize);
        pagination.TotalCount.ShouldBe(0);
        pagination.TotalPages.ShouldBe(0);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(3, 5)]
    [InlineData(6, 1)]
    [InlineData(-22, -33)]
    [InlineData(0, 0)]
    public async Task GetAllUrlDtoAsync_RetursPagination_WhenUrlsExist(int pageNumber, int pageSize)
    {
        // Arrange
        DbContext.Users.Add(_userEntity);
        DbContext.Urls.AddRange(_urlEntities);

        await DbContext.SaveChangesAsync();

        // Get expected values
        var expectedPageNumber = Math.Max(1, pageNumber);
        var expectedPageSize = Math.Max(1, pageSize);

        var totalCount = _urlEntities.Count;
        var totalPages = (int)Math.Ceiling((double)totalCount / expectedPageSize);
        var paginationCount = _urlEntities
            .Skip((expectedPageNumber - 1) * expectedPageSize)
            .Take(expectedPageSize)
            .Count();

        // Act
        var pagination = await UrlRepository.GetAllUrlDtoAsync(pageNumber, pageSize);

        // Assert
        pagination.ShouldNotBeNull();
        pagination.Items.Count().ShouldBe(paginationCount);
        pagination.PageNumber.ShouldBe(expectedPageNumber);
        pagination.PageSize.ShouldBe(expectedPageSize);
        pagination.TotalCount.ShouldBe(totalCount);
        pagination.TotalPages.ShouldBe(totalPages);
    }

    [Fact]
    public async Task GetUrlInfoDtoAsync_ShouldReturnNull_WhenUrlDoesNotExist()
    {
        // Arrange
        var urlId = Guid.NewGuid();

        // Act
        var urlInfo = await UrlRepository.GetUrlInfoDtoAsync(urlId);

        // Assert
        urlInfo.ShouldBeNull();
    }

    [Fact]
    public async Task GetUrlInfoDtoAsync_ShouldReturnUrlInfoAndAddUrlInfoToCache_WhenUrlExist()
    {
        // Arrange
        DbContext.Urls.AddRange(_urlEntities);
        DbContext.Users.Add(_userEntity);

        await DbContext.SaveChangesAsync();

        var urlToGet = _urlEntities[Random.Shared.Next(_urlEntities.Count)];

        // Act
        var urlInfo = await UrlRepository.GetUrlInfoDtoAsync(urlToGet.Id);

        // Assert
        MemoryCache.TryGetValue($"{CacheKeys.UrlId}-{urlToGet.Id}", out _).ShouldBeTrue();

        urlInfo.ShouldNotBeNull();
        urlInfo.CreatedAt.ShouldBe(urlToGet.CreatedAt);
        urlInfo.Id.ShouldBe(urlToGet.Id);
        urlInfo.ShortCode.ShouldBe(urlToGet.ShortCode);
        urlInfo.UrlOriginal.ShouldBe(urlToGet.UrlOriginal);
        urlInfo.UserId.ShouldBe(urlToGet.User.Id);
        urlInfo.User.Id.ShouldBe(urlToGet.User.Id);
    }

    [Fact]
    public async Task GetOriginalUrlByShortCodeAsync_ShouldReturnNull_WhenUrlByShortCodeDoesNotExist()
    {
        // Arrange
        var shortCode = _urlEntities[Random.Shared.Next(_urlEntities.Count)].ShortCode;

        // Act
        var originalUrl = await UrlRepository.GetOriginalUrlByShortCodeAsync(shortCode);

        // Assert
        originalUrl.ShouldBeNull();
    }

    [Fact]
    public async Task GetOriginalUrlByShortCodeAsync_ShouldReturnOriginalUrl_WhenUrlByShortCodeExist()
    {
        // Arrange
        DbContext.Urls.AddRange(_urlEntities);
        DbContext.Users.Add(_userEntity);

        await DbContext.SaveChangesAsync();

        var urlToGet = _urlEntities[Random.Shared.Next(_urlEntities.Count)];

        // Act
        var originalUrl = await UrlRepository.GetOriginalUrlByShortCodeAsync(urlToGet.ShortCode);

        // Assert
        originalUrl.ShouldNotBeNull();
        originalUrl.ShouldBe(urlToGet.UrlOriginal);
    }

    [Fact]
    public void AddUrl_ShouldChangeTheStateToAdded()
    {
        // Arrange
        var url = _urlEntities[Random.Shared.Next(_urlEntities.Count)];

        // Act
        UrlRepository.AddUrl(url);

        // Assert
        DbContext.Entry(url).State.ShouldBe(Microsoft.EntityFrameworkCore.EntityState.Added);
    }

    public async Task InitializeAsync()
    {
        _dbTransaction = await DbContext.Database.BeginTransactionAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbTransaction.RollbackAsync();
    }
}