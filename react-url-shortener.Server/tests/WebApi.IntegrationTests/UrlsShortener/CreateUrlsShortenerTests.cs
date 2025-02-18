using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using WebApi.Contracts;
using WebApi.Contracts.V1.Responses.Url;
using WebApi.IntegrationTests.Abstractions;
using WebApi.IntegrationTests.Extensions;

namespace WebApi.IntegrationTests.UrlsShortener;

public class CreateUrlsShortenerTests(IntegrationTestWebApplicationFactory factory) : BaseUrlsShortenerTests(factory)
{
    [Fact]
    public async Task Create_WhenUrlIsValid_ShouldCreateAndAddUrlToDatabase()
    {
        // Arrange
        await this.AuthenticateUserAsync(TestUsername, TestPassword);

        // Act
        var createdUrlResponse = await HttpClient.PostAsync($"{ApiRoutes.Urls.Create}?url={TestUrl}", null);

        // Assert
        createdUrlResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
        var createdUrlContent = await createdUrlResponse.Content.ReadFromJsonAsync<UrlResponse>();
        createdUrlContent.ShouldNotBeNull();

        var createdUrl = await DbContext.Urls.FirstOrDefaultAsync(u => u.Id == createdUrlContent.Id);
        createdUrl.ShouldNotBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("it's not url")]
    [InlineData("test")]
    [InlineData("https:/")]
    public async Task Create_WhenUrlIsInvalid_ShouldFail(string url)
    {
        // Arrange
        await this.AuthenticateUserAsync(TestUsername, TestPassword);

        // Act
        var createdUrlResponse = await HttpClient.PostAsync($"{ApiRoutes.Urls.Create}?url={url}", null);

        // Assert
        createdUrlResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_WhenUserIsNotAuthenticated_ShouldFail()
    {
        // Arrange

        // Act
        var createdUrlResponse = await HttpClient.PostAsync($"{ApiRoutes.Urls.Create}?url={TestUrl}", null);

        // Assert
        createdUrlResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.Unauthorized);
    }
}