using AutoFixture;
using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using MockQueryable.Moq;
using Moq;
using Shouldly;
using WebApi.DTOs.Identity;
using WebApi.Services.Identity;
using WebApi.UnitTests.Extensions;

namespace WebApi.UnitTests.Services.Identity;

public class IdentityServiceSignupTests : BaseIdentityTests
{
    private readonly UserEntity _user;
    private readonly SignupDto _signupDto;

    public IdentityServiceSignupTests()
    {
        _user = Fixture.Create<UserEntity>();
        _signupDto = Fixture.Build<SignupDto>()
            .With(s => s.Username, _user.UserName)
            .Create();
    }

    [Fact]
    public async Task SignupAsync_ShouldReturnFailure_WhenUserNameExist()
    {
        // Arrange
        List<UserEntity> userEntities = [_user];
        var userEntitiesDbSetMock = userEntities.BuildMockDbSet();

        UserManagerMock
            .Setup(userManager => userManager.Users)
            .Returns(userEntitiesDbSetMock.Object);

        // Act
        var signupResult = await IdentityService.SignupAsync(_signupDto);

        // Assert
        var failure = signupResult.Failure();
        failure.FailureCode.ShouldBe(Common.FailureCode.Conflict);
        failure.Errors.ShouldContain(IdentityServiceMessages.USERNAME_ALREADY_EXIST);
    }

    [Fact]
    public async Task SignupAsync_ShouldReturnFailure_WhenCreateAsyncFails()
    {
        // Arrange
        List<UserEntity> userEntities = [];
        var userEntitiesDbSetMock = userEntities.BuildMockDbSet();

        UserManagerMock
            .Setup(userManager => userManager.Users)
            .Returns(userEntitiesDbSetMock.Object);

        UserManagerMock
            .Setup(userManager => userManager.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError
            {
                Code = "SOME_ERROR_CODE",
                Description = "Some error occured",
            }));

        // Act
        var signupResult = await IdentityService.SignupAsync(_signupDto);

        // Assert
        var failure = signupResult.Failure();
        failure.FailureCode.ShouldBe(Common.FailureCode.BadRequest);
        failure.Errors.ShouldContain("Some error occured");

        UserManagerMock
            .Verify(userManager => userManager.CreateAsync(
                It.Is<UserEntity>(userEntity => userEntity.UserName == _signupDto.Username),
                It.Is<string>(s => s == _signupDto.Password)
            ), Times.Once);
    }

    [Fact]
    public async Task SignupAsync_ShouldReturnSuccess()
    {
        // Arrange
        List<UserEntity> userEntities = [];
        var userEntitiesDbSetMock = userEntities.BuildMockDbSet();

        UserManagerMock
            .Setup(userManager => userManager.Users)
            .Returns(userEntitiesDbSetMock.Object);

        UserManagerMock
            .Setup(userManager => userManager.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        UserManagerMock
            .Setup(userManager => userManager.GetRolesAsync(It.IsAny<UserEntity>()))
            .ReturnsAsync([]);

        // Act
        var signupResult = await IdentityService.SignupAsync(_signupDto);

        // Assert
        var success = signupResult.Success();
        success.RefreshToken.ShouldNotBeNullOrWhiteSpace();
        success.Token.ShouldNotBeNullOrWhiteSpace();
        success.User.Username.ShouldBe(_signupDto.Username);
        success.Roles.ShouldNotBeNull();

        UserManagerMock
            .Verify(userManager => userManager.CreateAsync(
                It.Is<UserEntity>(userEntity => userEntity.UserName == _signupDto.Username),
                It.Is<string>(s => s == _signupDto.Password)
            ), Times.Once);

        UserManagerMock
            .Verify(userManager => userManager.GetRolesAsync(
                It.Is<UserEntity>(userEntity => userEntity.UserName == _signupDto.Username)
            ), Times.Once);

        RefreshTokenRepositoryMock
            .Verify(refreshTokenRepository => refreshTokenRepository.AddRefreshToken(
                It.Is<RefreshTokenEntity>(refreshToken => refreshToken.User.UserName == _user.UserName)
            ), Times.Once);

        UnitOfWorkMock
            .Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}