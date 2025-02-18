using System.Net.Http.Json;
using Shouldly;
using WebApi.Contracts;
using WebApi.Contracts.V1.Responses.Url;
using WebApi.IntegrationTests.Abstractions;
using WebApi.IntegrationTests.Extensions;

namespace WebApi.IntegrationTests.UrlsShortener;

public class BaseUrlsShortenerTests(IntegrationTestWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    protected const string TestUsername = "test";
    protected const string TestPassword = "testtest";
    protected const string TestUrl = "https://google.com";

    protected async Task<HttpResponseMessage> CreateUrlAsync(string url)
    {
        var createdUrlResponse = await HttpClient.PostAsync($"{ApiRoutes.Urls.Create}?url={url}", null);
        createdUrlResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
        return createdUrlResponse;
    }

    protected async Task<UrlResponse> CreateAndGetUrlAsync(string url)
    {
        var createResponse = await CreateUrlAsync(url);
        var createdUrlCotnent = await createResponse.Content.ReadFromJsonAsync<UrlResponse>();
        createdUrlCotnent.ShouldNotBeNull();
        return createdUrlCotnent;
    }

    protected async Task<UrlResponse> AuthenticateUserCreateAndGetUrlAsync(string username, string password, string url)
    {
        await this.AuthenticateUserAsync(username, password);
        return await CreateAndGetUrlAsync(url);
    }

    protected async Task<HttpResponseMessage> AuthenticateUserAndCreateUrlAsync(string username, string password, string url)
    {
        await this.AuthenticateUserAsync(username, password);
        return await CreateUrlAsync(url);
    }
}