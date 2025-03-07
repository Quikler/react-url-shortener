using AutoFixture;
using Moq;
using Shouldly;

namespace WebAPI.UnitTests.Services.UrlShortener;

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
        urlOriginalResult.IsSuccess.ShouldBeFalse();
        var matchResult = urlOriginalResult.Match(
            success => throw new Exception("Should not be success."),
            failure => failure
        );

        matchResult.FailureCode.ShouldBe(WebApi.Common.FailureCode.NotFound);
        matchResult.Errors.ShouldContain("Url not found.");

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
        urlOriginalResult.IsSuccess.ShouldBeTrue();
        var matchResult = urlOriginalResult.Match(
            success => success,
            failure => throw new Exception("Should not be failure.")
        );

        matchResult.ShouldBe(_originalUrl);

        UrlRepositoryMock
            .Verify(urlRepository => urlRepository.GetOriginalUrlByShortCodeAsync(_shortCode), Times.Once);
    }
}