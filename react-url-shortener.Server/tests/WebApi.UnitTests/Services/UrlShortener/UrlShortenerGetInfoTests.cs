using AutoFixture;
using Moq;
using Shouldly;
using WebApi.DTOs.Url;
using WebApi.Services.UrlShortener;

namespace WebAPI.UnitTests.Services.UrlShortener;

public class UrlShortenerGetInfoTests : BaseUrlShortenerTests
{
    private readonly Guid _urlId;
    private readonly UrlInfoDto _urlInfoDto;

    public UrlShortenerGetInfoTests()
    {
        _urlId = Guid.NewGuid();

        _urlInfoDto = Fixture.Build<UrlInfoDto>()
            .With(u => u.Id, _urlId)
            .Create();
    }

    [Fact]
    public async Task GetInfoAsync_ShouldReturnFailure_WhenUrlNotExist()
    {
        // Arrange
        UrlRepositoryMock
            .Setup(urlRepository => urlRepository.GetUrlInfoDtoAsync(It.IsAny<Guid>()))
            .ReturnsAsync((UrlInfoDto?)null);

        // Act
        var infoResult = await UrlShortenerService.GetInfoAsync(_urlId);

        // Assert
        infoResult.IsSuccess.ShouldBeFalse();
        var matchResult = infoResult.Match(
            success => throw new Exception("Should not be success"),
            failure => failure
        );

        matchResult.FailureCode.ShouldBe(WebApi.Common.FailureCode.NotFound);
        matchResult.Errors.ShouldContain(UrlShortenerServiceMessages.URL_NOT_FOUND);

        UrlRepositoryMock
            .Verify(urlRepository => urlRepository.GetUrlInfoDtoAsync(_urlId), Times.Once);
    }

    [Fact]
    public async Task GetInfoAsync_ShouldReturnSuccess_WhenUrlExist()
    {
        // Arrange
        UrlRepositoryMock
            .Setup(urlRepository => urlRepository.GetUrlInfoDtoAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_urlInfoDto);

        // Act
        var infoResult = await UrlShortenerService.GetInfoAsync(_urlId);

        // Assert
        infoResult.IsSuccess.ShouldBeTrue();
        var matchResult = infoResult.Match(
            success => success,
            failure => throw new Exception("Should not be failure")
        );

        matchResult.Id.ShouldBe(_urlId);
        matchResult.CreatedAt.ShouldBe(_urlInfoDto.CreatedAt);
        matchResult.ShortCode.ShouldBe(_urlInfoDto.ShortCode);
        matchResult.UrlOriginal.ShouldBe(_urlInfoDto.UrlOriginal);
        matchResult.UserId.ShouldBe(_urlInfoDto.UserId);
        matchResult.User.ShouldBe(_urlInfoDto.User);

        UrlRepositoryMock
            .Verify(urlRepository => urlRepository.GetUrlInfoDtoAsync(_urlId), Times.Once);
    }
}