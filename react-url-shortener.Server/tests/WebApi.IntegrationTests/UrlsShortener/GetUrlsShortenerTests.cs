using System.Net.Http.Json;
using Shouldly;
using WebApi.Contracts;
using WebApi.Contracts.V1.Responses;
using WebApi.Contracts.V1.Responses.Url;
using WebApi.IntegrationTests.Abstractions;
using WebApi.IntegrationTests.Extensions;

namespace WebApi.IntegrationTests.UrlsShortener;

public class GetUrlsShortenerTests(IntegrationTestWebApplicationFactory factory) : BaseUrlsShortenerTests(factory)
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    public async Task GetAll_WhenUrlsExist_ShouldReturnCollectionWithCorrectNumberOfUrls(int numberOfUrls)
    {
        // Arrange
        await this.SignupAndAuthenticateUserAsync(TestUsername, TestPassword);

        List<UrlResponse> urlResponses = [];
        for (int i = 0; i < numberOfUrls; i++)
        {
            var url = $"{TestUrl}-{i}";
            var createdUrlResponse = await CreateAndGetUrlAsync(url);
            urlResponses.Add(createdUrlResponse);
        }

        // Act
        var getAllResponse = await HttpClient.GetAsync(ApiRoutes.Urls.GetAll);

        // Assert
        getAllResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

        var urls = await getAllResponse.Content.ReadFromJsonAsync<PaginationResponse<UrlResponse>>();
        urls.ShouldNotBeNull();
        urls.Items.Count().ShouldBe(numberOfUrls);
        urls.Items.ShouldBe(urlResponses);
    }

    [Fact]
    public async Task GetInfo_WhenUserIsNotAuthenticated_ShouldFail()
    {
        // Arrange
        var createdUrlResponse = await AuthenticateUserCreateAndGetUrlAsync(TestUsername, TestPassword, TestUrl);
        this.LogoutUser();

        // Act
        var getInfoRoute = ApiRoutes.Urls.GetInfo.Replace("{urlId}", createdUrlResponse.Id.ToString());
        var urlInfoResponse = await HttpClient.GetAsync(getInfoRoute);

        // Assert
        urlInfoResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetInfo_WhenUrlExist_ShouldReturnUrlInfo()
    {
        // Arrange
        var createdUrlResponse = await AuthenticateUserCreateAndGetUrlAsync(TestUsername, TestPassword, TestUrl);

        // Act
        var getInfoRoute = ApiRoutes.Urls.GetInfo.Replace("{urlId}", createdUrlResponse.Id.ToString());
        var urlInfoResponse = await HttpClient.GetAsync(getInfoRoute);

        // Assert
        urlInfoResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        var urlInfoContent = await urlInfoResponse.Content.ReadFromJsonAsync<UrlInfoResponse>();
        urlInfoContent.ShouldNotBeNull();
        urlInfoContent.Id.ShouldBe(createdUrlResponse.Id);
    }

    [Fact]
    public async Task GetInfo_WhenUrlDoesntExist_ShouldReturnNotFound()
    {
        // Arrange
        await this.SignupAndAuthenticateUserAsync(TestUsername, TestPassword);

        // Act
        var getInfoRoute = ApiRoutes.Urls.GetInfo.Replace("{urlId}", Guid.NewGuid().ToString());
        var urlInfoResponse = await HttpClient.GetAsync(getInfoRoute);

        // Assert
        urlInfoResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
    }
}