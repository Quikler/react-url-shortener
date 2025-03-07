using AutoFixture;
using Moq;
using Shouldly;
using WebApi.DTOs;
using WebApi.DTOs.Url;
using WebAPI.UnitTests.Services.UrlShortener;

namespace WebApi.UnitTests.Services.UrlShortener;

public class UrlShortenerGetAllTests : BaseUrlShortenerTests
{
    private readonly PaginationDto<UrlDto> _paginationDto;

    public UrlShortenerGetAllTests()
    {
        _paginationDto = Fixture.Create<PaginationDto<UrlDto>>();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnSuccess()
    {
        // Arrange
        UrlRepositoryMock
            .Setup(urlRepository => urlRepository.GetAllUrlDtoAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(_paginationDto);

        // Act
        var paginationResult = await UrlShortenerService.GetAllAsync(_paginationDto.PageNumber, _paginationDto.PageSize);

        // Assert
        paginationResult.IsSuccess.ShouldBeTrue();

        var matchResult = paginationResult.Match(success => success, failure => throw new Exception("Should not be failure"));

        matchResult.PageNumber.ShouldBe(_paginationDto.PageNumber);
        matchResult.PageSize.ShouldBe(_paginationDto.PageSize);
        matchResult.TotalPages.ShouldBe(_paginationDto.TotalPages);
        matchResult.TotalCount.ShouldBe(_paginationDto.TotalCount);
        matchResult.Items.ShouldBe(_paginationDto.Items);

        UrlRepositoryMock
            .Verify(urlRepository => urlRepository.GetAllUrlDtoAsync(_paginationDto.PageNumber, _paginationDto.PageSize), Times.Once);
    }
}