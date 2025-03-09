using AutoFixture;
using Moq;
using Shouldly;
using WebApi.DTOs.Url;
using WebApi.Services.UrlShortener;
using WebApi.UnitTests.Extensions;

namespace WebApi.UnitTests.Services.UrlShortener;

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
        var failure = infoResult.Failure();

        failure.FailureCode.ShouldBe(WebApi.Common.FailureCode.NotFound);
        failure.Errors.ShouldContain(UrlShortenerServiceMessages.URL_NOT_FOUND);

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
        var success = infoResult.Success();

        success.Id.ShouldBe(_urlId);
        success.CreatedAt.ShouldBe(_urlInfoDto.CreatedAt);
        success.ShortCode.ShouldBe(_urlInfoDto.ShortCode);
        success.UrlOriginal.ShouldBe(_urlInfoDto.UrlOriginal);
        success.UserId.ShouldBe(_urlInfoDto.UserId);
        success.User.ShouldBe(_urlInfoDto.User);

        UrlRepositoryMock
            .Verify(urlRepository => urlRepository.GetUrlInfoDtoAsync(_urlId), Times.Once);
    }
}