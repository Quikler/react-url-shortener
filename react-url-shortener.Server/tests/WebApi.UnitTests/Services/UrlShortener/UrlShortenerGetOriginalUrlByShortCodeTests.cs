using AutoFixture;
using Moq;
using Shouldly;
using WebApi.Services.UrlShortener;
using WebApi.UnitTests.Extensions;

namespace WebApi.UnitTests.Services.UrlShortener;

public class UrlShortenerGetOriginalUrlByShortCodeTests : BaseUrlShortenerTests
{
    private readonly string _shortCode;
    private readonly string _originalUrl;

    public UrlShortenerGetOriginalUrlByShortCodeTests()
    {
        _shortCode = Fixture.Create<string>();
        _originalUrl = Fixture.Create<string>();
    }

    [Fact]
    public async Task GetOriginalUrlByShortCodeAsync_ShouldReturnFailure_WhenUrlNotFound()
    {
        // Arrange
        UrlRepositoryMock
            .Setup(urlRepository => urlRepository.GetOriginalUrlByShortCodeAsync(It.IsAny<string>()))
            .ReturnsAsync((string?)null);

        // Act
        var urlOriginalResult = await UrlShortenerService.GetOriginalUrlByShortCodeAsync(_shortCode);

        // Assert
        var failure = urlOriginalResult.Failure();

        failure.FailureCode.ShouldBe(WebApi.Common.FailureCode.NotFound);
        failure.Errors.ShouldContain(UrlShortenerServiceMessages.URL_NOT_FOUND);

        UrlRepositoryMock
            .Verify(urlRepository => urlRepository.GetOriginalUrlByShortCodeAsync(_shortCode), Times.Once);
    }

    [Fact]
    public async Task GetOriginalUrlByShortCodeAsync_ShouldReturnSuccess_WhenUrlFound()
    {
        // Arrange
        UrlRepositoryMock
            .Setup(urlRepository => urlRepository.GetOriginalUrlByShortCodeAsync(It.IsAny<string>()))
            .ReturnsAsync(_originalUrl);

        // Act
        var urlOriginalResult = await UrlShortenerService.GetOriginalUrlByShortCodeAsync(_shortCode);

        // Assert
        var success = urlOriginalResult.Success();

        success.ShouldBe(_originalUrl);

        UrlRepositoryMock
            .Verify(urlRepository => urlRepository.GetOriginalUrlByShortCodeAsync(_shortCode), Times.Once);
    }
}