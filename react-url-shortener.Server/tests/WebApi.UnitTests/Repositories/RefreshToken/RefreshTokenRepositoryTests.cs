using AutoFixture;
using DAL.Entities;
using MockQueryable.Moq;
using Moq;
using Shouldly;

namespace WebApi.UnitTests.Repositories.RefreshToken;

public class RefreshTokenRepositoryTests : BaseRefreshTokenTests
{
    private readonly RefreshTokenEntity _refreshTokenEntity;

    public RefreshTokenRepositoryTests()
    {
        _refreshTokenEntity = Fixture.Create<RefreshTokenEntity>();
    }

    [Fact]
    public void AddRefreshToken_ShouldAddRefreshToken()
    {
        // Arrange
        List<RefreshTokenEntity> refreshTokenEntities = [];
        var refreshTokenDbSetMock = refreshTokenEntities.BuildMockDbSet();

        DbContextMock
            .Setup(dbContext => dbContext.RefreshTokens)
            .Returns(refreshTokenDbSetMock.Object);

        // Act
        RefreshTokenRepository.AddRefreshToken(_refreshTokenEntity);

        // Assert
        refreshTokenDbSetMock
            .Verify(refreshTokenDbSet => refreshTokenDbSet.Add(_refreshTokenEntity), Times.Once);
    }

    [Fact]
    public async Task GetRefreshTokenWithUserAsync_ShouldReturnNull_WhenTokenNotFound()
    {
        // Arrange
        List<RefreshTokenEntity> refreshTokenEntities = [];
        var refreshTokenDbSetMock = refreshTokenEntities.BuildMockDbSet();

        DbContextMock
            .Setup(dbContext => dbContext.RefreshTokens)
            .Returns(refreshTokenDbSetMock.Object);

        // Act
        var refreshTokenWithUser = await RefreshTokenRepository.GetRefreshTokenWithUserAsync(_refreshTokenEntity.Token, false);

        // Assert
        refreshTokenWithUser.ShouldBeNull();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetRefreshTokenWithUserAsync_ShouldReturnRefreshToken_WhenTokenFound(bool tracking)
    {
        // Arrange
        List<RefreshTokenEntity> refreshTokenEntities = [_refreshTokenEntity];
        var refreshTokenDbSetMock = refreshTokenEntities.BuildMockDbSet();

        DbContextMock
            .Setup(dbContext => dbContext.RefreshTokens)
            .Returns(refreshTokenDbSetMock.Object);

        // Act
        var refreshTokenWithUser = await RefreshTokenRepository.GetRefreshTokenWithUserAsync(_refreshTokenEntity.Token, tracking);

        // Assert
        refreshTokenWithUser.ShouldBe(_refreshTokenEntity);

        // if traking then invocations should be 3, otherwise 4
        refreshTokenDbSetMock.Invocations.Count.ShouldBe(tracking ? 3 : 4);
    }
}