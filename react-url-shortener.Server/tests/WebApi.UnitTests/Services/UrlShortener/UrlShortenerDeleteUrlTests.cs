using Moq;
using Shouldly;
using WebApi.Services.UrlShortener;
using WebApi.UnitTests.Extensions;

namespace WebApi.UnitTests.Services.UrlShortener;

public class UrlShortenerDeleteUrlTests : BaseUrlShortenerTests
{
    private readonly Guid _urlId;
    private readonly Guid _userId;
    private readonly string[] _userRoles;

    public UrlShortenerDeleteUrlTests()
    {
        _urlId = Guid.NewGuid();
        _userId = Guid.NewGuid();
        _userRoles = [];
    }

    [Fact]
    public async Task DeleteUrlAsync_ShouldReturnFailure_WhenUrlByIdDoesNotExist()
    {
        // Arrange
        UrlRepositoryMock
            .Setup(urlRepository => urlRepository.IsUrlByIdExistAsync(It.IsAny<Guid>()))
            .ReturnsAsync(false);

        // Act
        var deleteResult = await UrlShortenerService.DeleteUrlAsync(_urlId, _userId, _userRoles);

        // Assert
        var failure = deleteResult.Failure();

        failure.FailureCode.ShouldBe(WebApi.Common.FailureCode.NotFound);
        failure.Errors.ShouldContain(UrlShortenerServiceMessages.URL_NOT_FOUND);

        UrlRepositoryMock
            .Verify(urlRepository => urlRepository.IsUrlByIdExistAsync(_urlId), Times.Once);
    }

    [Fact]
    public async Task DeleteUrlAsync_ShouldReturnFailure_WhenUserNotOwnerOrNotAdmin()
    {
        // Arrange
        UrlRepositoryMock
            .Setup(urlRepository => urlRepository.IsUrlByIdExistAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);

        UrlRepositoryMock
            .Setup(urlRepository => urlRepository.IsUserOwnerOrAdminAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string[]>()))
            .ReturnsAsync(false);

        // Act
        var deleteResult = await UrlShortenerService.DeleteUrlAsync(_urlId, _userId, _userRoles);

        // Assert
        var failure = deleteResult.Failure();

        failure.FailureCode.ShouldBe(WebApi.Common.FailureCode.Forbidden);
        failure.Errors.ShouldContain(UrlShortenerServiceMessages.USER_DOESNT_AUTHORIZED_TO_URL);

        UrlRepositoryMock
            .Verify(urlRepository => urlRepository.IsUrlByIdExistAsync(_urlId), Times.Once);

        UrlRepositoryMock
            .Verify(urlRepository => urlRepository.IsUserOwnerOrAdminAsync(_userId, _urlId, _userRoles), Times.Once);
    }

    [Fact]
    public async Task DeleteUrlAsync_ShouldReturnFailure_WhenDeleteUrlAsyncReturnsZero()
    {
        // Arrange
        UrlRepositoryMock
            .Setup(urlRepository => urlRepository.IsUrlByIdExistAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);

        UrlRepositoryMock
            .Setup(urlRepository => urlRepository.IsUserOwnerOrAdminAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string[]>()))
            .ReturnsAsync(true);

        UrlRepositoryMock
            .Setup(urlRepository => urlRepository.DeleteUrlAsync(It.IsAny<Guid>()))
            .ReturnsAsync(0);

        // Act
        var deleteResult = await UrlShortenerService.DeleteUrlAsync(_urlId, _userId, _userRoles);

        // Assert
        var failure = deleteResult.Failure();

        failure.FailureCode.ShouldBe(WebApi.Common.FailureCode.BadRequest);
        failure.Errors.ShouldContain(UrlShortenerServiceMessages.CANNOT_DELETE_URL);

        UrlRepositoryMock
            .Verify(urlRepository => urlRepository.IsUrlByIdExistAsync(_urlId), Times.Once);

        UrlRepositoryMock
            .Verify(urlRepository => urlRepository.IsUserOwnerOrAdminAsync(_userId, _urlId, _userRoles), Times.Once);

        UrlRepositoryMock
            .Verify(urlRepository => urlRepository.DeleteUrlAsync(_urlId), Times.Once);
    }

    [Fact]
    public async Task DeleteUrlAsync_ShouldReturnSuccess()
    {
        // Arrange
        UrlRepositoryMock
            .Setup(urlRepository => urlRepository.IsUrlByIdExistAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);

        UrlRepositoryMock
            .Setup(urlRepository => urlRepository.IsUserOwnerOrAdminAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string[]>()))
            .ReturnsAsync(true);

        UrlRepositoryMock
            .Setup(urlRepository => urlRepository.DeleteUrlAsync(It.IsAny<Guid>()))
            .ReturnsAsync(1);

        // Act
        var deleteResult = await UrlShortenerService.DeleteUrlAsync(_urlId, _userId, _userRoles);

        // Assert
        deleteResult.Success();

        UrlRepositoryMock
            .Verify(urlRepository => urlRepository.IsUrlByIdExistAsync(_urlId), Times.Once);

        UrlRepositoryMock
            .Verify(urlRepository => urlRepository.IsUserOwnerOrAdminAsync(_userId, _urlId, _userRoles), Times.Once);

        UrlRepositoryMock
            .Verify(urlRepository => urlRepository.DeleteUrlAsync(_urlId), Times.Once);
    }
}