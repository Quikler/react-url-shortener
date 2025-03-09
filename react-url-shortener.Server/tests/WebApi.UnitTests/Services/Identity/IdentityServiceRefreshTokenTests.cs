using AutoFixture;
using DAL.Entities;
using Moq;
using Shouldly;
using WebApi.Services.Identity;
using WebApi.UnitTests.Extensions;

namespace WebApi.UnitTests.Services.Identity;

public class IdentityServiceRefreshTokenTests : BaseIdentityTests
{
    private readonly UserEntity _user;
    private readonly RefreshTokenEntity _refreshTokenEntity;
    private readonly string _refreshToken;

    public IdentityServiceRefreshTokenTests()
    {
        _user = Fixture.Create<UserEntity>();
        _refreshTokenEntity = Fixture.Build<RefreshTokenEntity>()
            .With(r => r.User, _user)
            .With(r => r.UserId, _user.Id)
            .With(r => r.ExpiryDate, DateTime.UtcNow.AddDays(180))
            .Create();
        _refreshToken = _refreshTokenEntity.Token;
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldReturnFailure_WhenRefreshTokenNotFound()
    {
        // Arrange
        RefreshTokenRepositoryMock
            .Setup(refreshTokenRepository => refreshTokenRepository.GetRefreshTokenWithUserAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync((RefreshTokenEntity?)null);

        // Act
        var refreshTokenResult = await IdentityService.RefreshTokenAsync(_refreshToken);

        // Assert
        var failure = refreshTokenResult.Failure();
        failure.FailureCode.ShouldBe(Common.FailureCode.Unauthorized);
        failure.Errors.ShouldContain(IdentityServiceMessages.REFRESH_TOKEN_EXPIRED);

        RefreshTokenRepositoryMock
            .Verify(refreshTokenRepository => refreshTokenRepository.GetRefreshTokenWithUserAsync(_refreshToken, true), Times.Once);
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldReturnFailure_WhenRefreshTokenExpired()
    {
        // Arrange
        _refreshTokenEntity.ExpiryDate = DateTime.UtcNow - TimeSpan.FromDays(1);

        RefreshTokenRepositoryMock
            .Setup(refreshTokenRepository => refreshTokenRepository.GetRefreshTokenWithUserAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(_refreshTokenEntity);

        // Act
        var refreshTokenResult = await IdentityService.RefreshTokenAsync(_refreshToken);

        // Assert
        var failure = refreshTokenResult.Failure();
        failure.FailureCode.ShouldBe(Common.FailureCode.Unauthorized);
        failure.Errors.ShouldContain(IdentityServiceMessages.REFRESH_TOKEN_EXPIRED);

        RefreshTokenRepositoryMock
            .Verify(refreshTokenRepository => refreshTokenRepository.GetRefreshTokenWithUserAsync(_refreshToken, true), Times.Once);
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldReturnFailure_WhenUserIsNull()
    {
        // Arrange
        _refreshTokenEntity.User = null!;

        RefreshTokenRepositoryMock
            .Setup(refreshTokenRepository => refreshTokenRepository.GetRefreshTokenWithUserAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(_refreshTokenEntity);

        // Act
        var refreshTokenResult = await IdentityService.RefreshTokenAsync(_refreshToken);

        // Assert
        var failure = refreshTokenResult.Failure();
        failure.FailureCode.ShouldBe(Common.FailureCode.Unauthorized);
        failure.Errors.ShouldContain(IdentityServiceMessages.USER_NOT_FOUND);

        RefreshTokenRepositoryMock
            .Verify(refreshTokenRepository => refreshTokenRepository.GetRefreshTokenWithUserAsync(_refreshToken, true), Times.Once);
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldReturnSuccess()
    {
        // Arrange
        RefreshTokenRepositoryMock
            .Setup(refreshTokenRepository => refreshTokenRepository.GetRefreshTokenWithUserAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(_refreshTokenEntity);

        UserManagerMock
            .Setup(userManager => userManager.GetRolesAsync(It.IsAny<UserEntity>()))
            .ReturnsAsync([]);

        // Act
        var refreshTokenResult = await IdentityService.RefreshTokenAsync(_refreshToken);

        // Assert
        var success = refreshTokenResult.Success();
        success.RefreshToken.ShouldNotBeNullOrWhiteSpace();
        success.Token.ShouldNotBeNullOrWhiteSpace();
        success.User.Username.ShouldBe(_user.UserName);
        success.Roles.ShouldNotBeNull();

        RefreshTokenRepositoryMock
            .Verify(refreshTokenRepository => refreshTokenRepository.GetRefreshTokenWithUserAsync(_refreshToken, true), Times.Once);

        UnitOfWorkMock
            .Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        UserManagerMock
            .Verify(userManager => userManager.GetRolesAsync(
                It.Is<UserEntity>(userEntity => userEntity.UserName == _user.UserName)
            ), Times.Once);
    }
}