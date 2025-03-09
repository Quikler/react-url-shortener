using AutoFixture;
using DAL.Entities;
using Moq;
using Shouldly;
using WebApi.Services.UrlShortener;
using WebApi.UnitTests.Extensions;

namespace WebApi.UnitTests.Services.UrlShortener;

public class UrlShortenerCreateShortenUrlTests : BaseUrlShortenerTests
{
    private readonly string _originalUrl;
    private readonly Guid _userId;

    public UrlShortenerCreateShortenUrlTests()
    {
        _originalUrl = Fixture.Create<string>();
        _userId = Guid.NewGuid();
    }

    [Fact]
    public async Task CreateShortenUrlAsync_ShouldReturnFailure_WhenUrlExist()
    {
        // Arrange
        UrlRepositoryMock
            .Setup(urlRepository => urlRepository.IsUrlOriginalExistAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        var createResult = await UrlShortenerService.CreateShortenUrlAsync(_originalUrl, _userId);

        // Assert
        var failure = createResult.Failure();

        failure.FailureCode.ShouldBe(WebApi.Common.FailureCode.Conflict);
        failure.Errors.ShouldContain(UrlShortenerServiceMessages.URL_ALREADY_EXIST);

        UrlRepositoryMock
            .Verify(urlRepository => urlRepository.IsUrlOriginalExistAsync(_originalUrl), Times.Once);
    }

    [Fact]
    public async Task CreateShortenUrlAsync_ShouldReturnFailure_WhenSaveChangesAsyncReturnsZero()
    {
        // Arrange
        UrlRepositoryMock
            .Setup(urlRepository => urlRepository.IsUrlOriginalExistAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        UnitOfWorkMock
            .Setup(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        var createResult = await UrlShortenerService.CreateShortenUrlAsync(_originalUrl, _userId);

        // Assert
        var failure = createResult.Failure();

        failure.FailureCode.ShouldBe(WebApi.Common.FailureCode.BadRequest);
        failure.Errors.ShouldContain(UrlShortenerServiceMessages.CANNOT_CREATE_URL);

        UrlRepositoryMock
            .Verify(urlRepository => urlRepository.IsUrlOriginalExistAsync(_originalUrl), Times.Once);

        UrlRepositoryMock
            .Verify(urlRepository => urlRepository.AddUrl(It.Is<UrlEntity>(
                urlEntity => urlEntity.UrlOriginal == _originalUrl && urlEntity.UserId == _userId)));

        UnitOfWorkMock
            .Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task CreateShortenUrlAsync_ShouldReturnSuccess()
    {
        // Arrange
        UrlRepositoryMock
            .Setup(urlRepository => urlRepository.IsUrlOriginalExistAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        UnitOfWorkMock
            .Setup(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var createResult = await UrlShortenerService.CreateShortenUrlAsync(_originalUrl, _userId);

        // Assert
        var success = createResult.Success();

        success.UrlOriginal.ShouldBe(_originalUrl);
        success.UserId.ShouldBe(_userId);

        UrlRepositoryMock
            .Verify(urlRepository => urlRepository.IsUrlOriginalExistAsync(_originalUrl), Times.Once);

        UrlRepositoryMock
            .Verify(urlRepository => urlRepository.AddUrl(It.Is<UrlEntity>(
                urlEntity => urlEntity.UrlOriginal == _originalUrl && urlEntity.UserId == _userId)));

        UnitOfWorkMock
            .Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()));
    }
}