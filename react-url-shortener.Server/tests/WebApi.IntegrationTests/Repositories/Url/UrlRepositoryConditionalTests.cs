using AutoFixture;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using Shouldly;
using WebApi.Repositories.Url;

namespace WebApi.IntegrationTests.Repositories.Url;

[Collection("SharedTestUrlCollection")]
public class UrlRepositoryConditionalTests : IAsyncLifetime
{
    private AppDbContext DbContext { get; }
    private UrlRepository UrlRepository { get; }
    private Fixture Fixture { get; }

    private IDbContextTransaction _dbTransaction = default!;

    private readonly UserEntity _userEntity;
    private readonly List<UrlEntity> _urlEntities;

    public UrlRepositoryConditionalTests(BaseUrlRepositoryTests baseUrlRepositoryTests)
    {
        DbContext = baseUrlRepositoryTests.DbContext;
        UrlRepository = baseUrlRepositoryTests.UrlRepository;
        Fixture = baseUrlRepositoryTests.Fixture;

        _userEntity = Fixture.Create<UserEntity>();

        _urlEntities = [.. Fixture.Build<UrlEntity>()
            .With(u => u.User, _userEntity)
            .With(u => u.UserId, _userEntity.Id)
            .CreateMany(5)];

        _userEntity.Urls = _urlEntities;
    }

    [Fact]
    public async Task IsUrlOriginalExistAsync_ShouldReturnFalse_WhenUrlDoesNotExist()
    {
        // Arrange
        var url = _urlEntities[Random.Shared.Next(_urlEntities.Count)];

        // Act
        var exist = await UrlRepository.IsUrlOriginalExistAsync(url.UrlOriginal);

        // Assert
        exist.ShouldBeFalse();
    }

    [Fact]
    public async Task IsUrlOriginalExistAsync_ShouldReturnTrue_WhenUrlExist()
    {
        // Arrange
        DbContext.Urls.AddRange(_urlEntities);
        DbContext.Users.Add(_userEntity);

        await DbContext.SaveChangesAsync();

        var url = _urlEntities[Random.Shared.Next(_urlEntities.Count)];

        // Act
        var exist = await UrlRepository.IsUrlOriginalExistAsync(url.UrlOriginal);

        // Assert
        exist.ShouldBeTrue();
    }

    [Fact]
    public async Task IsUrlByIdExistAsync_ShouldReturnFalse_WhenUrlDoesNotExist()
    {
        // Arrange
        var url = _urlEntities[Random.Shared.Next(_urlEntities.Count)];

        // Act
        var exist = await UrlRepository.IsUrlByIdExistAsync(url.Id);

        // Assert
        exist.ShouldBeFalse();
    }

    [Fact]
    public async Task IsUrlByIdExistAsync_ShouldReturnTrue_WhenUrlExist()
    {
        // Arrange
        DbContext.Urls.AddRange(_urlEntities);
        DbContext.Users.Add(_userEntity);

        await DbContext.SaveChangesAsync();

        var url = _urlEntities[Random.Shared.Next(_urlEntities.Count)];

        // Act
        var exist = await UrlRepository.IsUrlByIdExistAsync(url.Id);

        // Assert
        exist.ShouldBeTrue();
    }

    [Fact]
    public async Task IsUserOwnsUrlAsync_ShouldReturnFalse_WhenUrlDoesNotExist()
    {
        // Arrange
        var url = _urlEntities[Random.Shared.Next(_urlEntities.Count)];

        // Act
        var exist = await UrlRepository.IsUserOwnsUrlAsync(_userEntity.Id, url.Id);

        // Assert
        exist.ShouldBeFalse();
    }

    [Fact]
    public async Task IsUserOwnsUrlAsync_ShouldReturnFalse_WhenUserNotOwnsUrl()
    {
        // Arrange
        DbContext.Urls.AddRange(_urlEntities);
        DbContext.Users.Add(_userEntity);

        await DbContext.SaveChangesAsync();

        var url = _urlEntities[Random.Shared.Next(_urlEntities.Count)];

        // Act
        var exist = await UrlRepository.IsUserOwnsUrlAsync(Guid.NewGuid(), url.Id);

        // Assert
        exist.ShouldBeFalse();
    }

    [Fact]
    public async Task IsUserOwnsUrlAsync_ShouldReturnTrue_WhenUserOwnsUrl()
    {
        // Arrange
        DbContext.Urls.AddRange(_urlEntities);
        DbContext.Users.Add(_userEntity);

        await DbContext.SaveChangesAsync();

        var url = _urlEntities[Random.Shared.Next(_urlEntities.Count)];

        // Act
        var exist = await UrlRepository.IsUserOwnsUrlAsync(_userEntity.Id, url.Id);

        // Assert
        exist.ShouldBeTrue();
    }

    [Fact]
    public async Task IsUserOwnerOrAdminAsync_ShouldReturnFalse_WhenUrlDoesNotExist()
    {
        // Arrange
        var url = _urlEntities[Random.Shared.Next(_urlEntities.Count)];

        // Act
        var ownerOrAdmin = await UrlRepository.IsUserOwnerOrAdminAsync(_userEntity.Id, url.Id, ["Admin"]);

        // Assert
        ownerOrAdmin.ShouldBeFalse();
    }

    [Fact]
    public async Task IsUserOwnerOrAdminAsync_ShouldReturnFalse_WhenUserNotOwnerOrAdmin()
    {
        // Arrange
        DbContext.Urls.AddRange(_urlEntities);
        DbContext.Users.Add(_userEntity);

        await DbContext.SaveChangesAsync();

        var url = _urlEntities[Random.Shared.Next(_urlEntities.Count)];

        // Act
        var ownerOrAdmin = await UrlRepository.IsUserOwnerOrAdminAsync(Guid.NewGuid(), url.Id, []);

        // Assert
        ownerOrAdmin.ShouldBeFalse();
    }

    [Fact]
    public async Task IsUserOwnerOrAdminAsync_ShouldReturnTrue_WhenUserOwner()
    {
        // Arrange
        DbContext.Urls.AddRange(_urlEntities);
        DbContext.Users.Add(_userEntity);

        await DbContext.SaveChangesAsync();

        var url = _urlEntities[Random.Shared.Next(_urlEntities.Count)];

        // Act
        var ownerOrAdmin = await UrlRepository.IsUserOwnerOrAdminAsync(_userEntity.Id, url.Id, []);

        // Assert
        ownerOrAdmin.ShouldBeTrue();
    }

    [Fact]
    public async Task IsUserOwnerOrAdminAsync_ShouldReturnTrue_WhenUserAdmin()
    {
        // Arrange
        DbContext.Urls.AddRange(_urlEntities);
        DbContext.Users.Add(_userEntity);

        await DbContext.SaveChangesAsync();

        var url = _urlEntities[Random.Shared.Next(_urlEntities.Count)];

        // Act
        var ownerOrAdmin = await UrlRepository.IsUserOwnerOrAdminAsync(Guid.NewGuid(), url.Id, ["Admin"]);

        // Assert
        ownerOrAdmin.ShouldBeTrue();
    }

    public async Task InitializeAsync()
    {
        _dbTransaction = await DbContext.Database.BeginTransactionAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbTransaction.RollbackAsync();
        await _dbTransaction.DisposeAsync();
    }
}