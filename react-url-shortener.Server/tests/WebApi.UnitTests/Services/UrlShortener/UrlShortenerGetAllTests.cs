using AutoFixture;
using Moq;
using Shouldly;
using WebApi.DTOs;
using WebApi.DTOs.Url;
using WebApi.UnitTests.Extensions;

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
        var success = paginationResult.Success();

        success.PageNumber.ShouldBe(_paginationDto.PageNumber);
        success.PageSize.ShouldBe(_paginationDto.PageSize);
        success.TotalPages.ShouldBe(_paginationDto.TotalPages);
        success.TotalCount.ShouldBe(_paginationDto.TotalCount);
        success.Items.ShouldBe(_paginationDto.Items);

        UrlRepositoryMock
            .Verify(urlRepository => urlRepository.GetAllUrlDtoAsync(_paginationDto.PageNumber, _paginationDto.PageSize), Times.Once);
    }
}