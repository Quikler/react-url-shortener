using AutoFixture;
using DAL.Entities;
using Moq;
using Shouldly;
using WebApi.DTOs.Identity;
using WebApi.Services.Identity;
using WebApi.UnitTests.Extensions;

namespace WebApi.UnitTests.Services.Identity;

public class IdentityServiceLoginTests : BaseIdentityTests
{
    private readonly UserEntity _user;
    private readonly LoginDto _loginDto;

    public IdentityServiceLoginTests()
    {
        _user = Fixture.Create<UserEntity>();
        _loginDto = Fixture.Build<LoginDto>()
            .With(s => s.Username, _user.UserName)
            .Create();
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFailure_WhenUserNameNotFound()
    {
        // Arrange
        UserManagerMock
            .Setup(userManager => userManager.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((UserEntity?)null);

        // Act
        var loginResult = await IdentityService.LoginAsync(_loginDto);

        // Assert
        var failure = loginResult.Failure();
        failure.FailureCode.ShouldBe(Common.FailureCode.Unauthorized);
        failure.Errors.ShouldContain(IdentityServiceMessages.INVALID_USERNAME_OR_PASSWORD);

        UserManagerMock
            .Verify(userManager => userManager.FindByNameAsync(_loginDto.Username), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFailure_WhenCheckPasswordAsyncFails()
    {
        // Arrange
        UserManagerMock
            .Setup(userManager => userManager.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(_user);

        UserManagerMock
            .Setup(userManager => userManager.CheckPasswordAsync(It.IsAny<UserEntity>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        var loginResult = await IdentityService.LoginAsync(_loginDto);

        // Assert
        var failure = loginResult.Failure();
        failure.FailureCode.ShouldBe(Common.FailureCode.Unauthorized);
        failure.Errors.ShouldContain(IdentityServiceMessages.INVALID_USERNAME_OR_PASSWORD);

        UserManagerMock
            .Verify(userManager => userManager.FindByNameAsync(_loginDto.Username), Times.Once);

        UserManagerMock
            .Verify(userManger => userManger.CheckPasswordAsync(_user, _loginDto.Password), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnSuccess()
    {
        // Arrange
        UserManagerMock
            .Setup(userManager => userManager.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(_user);

        UserManagerMock
            .Setup(userManager => userManager.CheckPasswordAsync(It.IsAny<UserEntity>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        UserManagerMock
            .Setup(userManager => userManager.GetRolesAsync(It.IsAny<UserEntity>()))
            .ReturnsAsync([]);

        // Act
        var loginResult = await IdentityService.LoginAsync(_loginDto);

        // Assert
        var success = loginResult.Success();
        success.RefreshToken.ShouldNotBeNullOrWhiteSpace();
        success.Token.ShouldNotBeNullOrWhiteSpace();
        success.User.Username.ShouldBe(_loginDto.Username);
        success.Roles.ShouldNotBeNull();

        UserManagerMock
            .Verify(userManager => userManager.FindByNameAsync(_loginDto.Username), Times.Once);

        UserManagerMock
            .Verify(userManger => userManger.CheckPasswordAsync(_user, _loginDto.Password), Times.Once);

        UserManagerMock
            .Verify(userManager => userManager.GetRolesAsync(
                It.Is<UserEntity>(userEntity => userEntity.UserName == _loginDto.Username)
            ), Times.Once);

        RefreshTokenRepositoryMock
            .Verify(refreshTokenRepository => refreshTokenRepository.AddRefreshToken(
                It.Is<RefreshTokenEntity>(refreshToken => refreshToken.UserId == _user.Id)
            ), Times.Once);

        UnitOfWorkMock
            .Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}