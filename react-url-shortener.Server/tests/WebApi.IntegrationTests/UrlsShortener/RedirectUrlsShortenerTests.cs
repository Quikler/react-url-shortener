using System.Net.Http.Json;
using Shouldly;
using WebApi.Contracts.V1.Responses.Url;
using WebApi.IntegrationTests.Abstractions;

namespace WebApi.IntegrationTests.UrlsShortener;

public class RedirectUrlsShortenerTests(IntegrationTestWebApplicationFactory factory) : BaseUrlsShortenerTests(factory)
{
    [Fact]
    public async Task RedirectToOriginal_WhenUrlShortCodeDoesntExist_ShouldReturnNotFound()
    {
        // Arrange

        // Act
        var urlRedirectResponse = await HttpClient.GetAsync("/123456");

        // Assert
        urlRedirectResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RedirectToOriginal_WhenUrlShortCodeExist_ShouldRedirect()
    {
        // Arrange
        HttpResponseMessage createdUrlResponse = await AuthenticateUserAndCreateUrlAsync(TestUsername, TestPassword, TestUrl);
        createdUrlResponse.Headers.Location.ShouldNotBeNull();
        var createdUrlContent = await createdUrlResponse.Content.ReadFromJsonAsync<UrlResponse>();
        createdUrlContent.ShouldNotBeNull();

        // Act
        var urlRedirectResponse = await HttpClient.GetAsync(createdUrlResponse.Headers.Location.AbsoluteUri);

        // Assert
        urlRedirectResponse.RequestMessage.ShouldNotBeNull();
        urlRedirectResponse.RequestMessage.RequestUri.ShouldNotBeNull();

        Uri requestUri = urlRedirectResponse.RequestMessage.RequestUri;
        Uri urlOriginal = new(createdUrlContent.UrlOriginal);

        // If requestUri is equal to urlOriginal it means the Redirect method was called to the originalUrl
        var urlsEqual = Uri.Compare(requestUri, urlOriginal, UriComponents.Host, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase);
        urlsEqual.ShouldBe(0);
    }
}