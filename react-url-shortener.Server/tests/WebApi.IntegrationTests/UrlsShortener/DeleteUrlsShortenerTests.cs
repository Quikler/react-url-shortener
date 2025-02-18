using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using WebApi.Contracts;
using WebApi.Contracts.V1.Responses.Url;
using WebApi.IntegrationTests.Abstractions;
using WebApi.IntegrationTests.Extensions;

namespace WebApi.IntegrationTests.UrlsShortener;

public class DeleteUrlsShortenerTests(IntegrationTestWebApplicationFactory factory) : BaseUrlsShortenerTests(factory)
{
    [Fact]
    public async Task Delete_WhenUrlExist_ShouldDeleteUrl()
    {
        // Arrange
        var createdUrl = await AuthenticateUserCreateAndGetUrlAsync(TestUsername, TestPassword, TestUrl);

        var urlDeletePath = ApiRoutes.Urls.Delete.Replace("{urlId}", createdUrl.Id.ToString());

        // Act
        var deletedUrlResponse = await HttpClient.DeleteAsync(urlDeletePath);

        // Assert
        deletedUrlResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);

        var deletedUrl = await DbContext.Urls.FirstOrDefaultAsync(u => u.Id == createdUrl.Id);
        deletedUrl.ShouldBeNull();
    }

    [Fact]
    public async Task Delete_WhenUserIsAdmin_ShouldDeleteUrl()
    {
        // Arrange
        var createdUrlResponse = await AuthenticateUserCreateAndGetUrlAsync(TestUsername, TestPassword, TestUrl);
        this.LogoutUser();

        await this.LoginAndAuthenticateUserAsync("Admin", "Admin123!");
        var urlDeletePath = ApiRoutes.Urls.Delete.Replace("{urlId}", createdUrlResponse.Id.ToString());

        // Act
        var deletedUrlResponse = await HttpClient.DeleteAsync(urlDeletePath);
        var deletedUrlContent = await deletedUrlResponse.Content.ReadAsStringAsync();

        // Assert
        deletedUrlResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent, deletedUrlContent);
    }

    [Fact]
    public async Task Delete_WhenUrlNotExist_ShouldReturnNotFound()
    {
        // Arrange
        await this.SignupAndAuthenticateUserAsync(TestUsername, TestPassword);
        var urlDeletePath = ApiRoutes.Urls.Delete.Replace("{urlId}", Guid.NewGuid().ToString());

        // Act
        var deletedUrlResponse = await HttpClient.DeleteAsync(urlDeletePath);

        // Assert
        deletedUrlResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var createdUrlResponse = await AuthenticateUserCreateAndGetUrlAsync(TestUsername, TestPassword, TestUrl);
        this.LogoutUser();
        var urlDeletePath = ApiRoutes.Urls.Delete.Replace("{urlId}", createdUrlResponse.Id.ToString());

        // Act
        var deletedUrlResponse = await HttpClient.DeleteAsync(urlDeletePath);

        // Assert
        deletedUrlResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.Unauthorized);
    }
}