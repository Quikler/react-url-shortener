using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shouldly;
using WebApi.Contracts;
using WebApi.Contracts.V1.Responses;
using WebApi.DTOs;
using WebApi.IntegrationTests.Abstractions;
using WebApi.IntegrationTests.Extensions;
using WebApi.Services.About;

namespace WebApi.IntegrationTests.About;

public class AboutIntegrationTests : BaseIntegrationTest
{
    private readonly WebApplicationFactory<Program> _mockedAboutServiceFactory;
    private readonly HttpClient _mockedAboutServiceHttpClient;

    public AboutIntegrationTests(IntegrationTestWebApplicationFactory factory) : base(factory)
    {
        _mockedAboutServiceFactory = Factory.WithWebHostBuilder(builder =>
        {
            var mockAboutService = new Mock<IAboutService>();
            mockAboutService
                .Setup(s => s.GetAboutAsync())
                .ReturnsAsync(FailureDto.BadRequest("Cannot update about."));

            builder.ConfigureTestServices(services =>
            {
                var aboutServiceDescriptor = services
                    .SingleOrDefault(d => d.ServiceType == typeof(IAboutService));

                if (aboutServiceDescriptor is not null)
                {
                    services.Remove(aboutServiceDescriptor);
                }

                services.AddScoped(_ => mockAboutService.Object);
            });
        });

        _mockedAboutServiceHttpClient = _mockedAboutServiceFactory.CreateClient();
    }

    [Fact]
    public async Task GetAbout_WhenFileExist_ReturnsAbout()
    {
        // Arrange

        // Act
        var aboutResponse = await HttpClient.GetAsync(ApiRoutes.About.Get);

        // Assert
        aboutResponse.EnsureSuccessStatusCode();
        var responseString = await aboutResponse.Content.ReadAsStringAsync();
        responseString.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetAbout_WhenSomethingGoesWrong_ReturnsBadRequest()
    {
        // Arrange

        // Act
        var aboutResponse = await _mockedAboutServiceHttpClient.GetAsync(ApiRoutes.About.Get);
        aboutResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);

        // Assert
        var failureResponse = await aboutResponse.Content.ReadFromJsonAsync<FailureResponse>();
        failureResponse.ShouldNotBeNull();
        failureResponse.Errors.ShouldContain("Cannot update about.");
    }

    [Fact]
    public async Task UpdateAbout_WhenUserAdmin_UpdatesAbout()
    {
        // Arrange
        await this.LoginAndAuthenticateUserAsync("Admin", "Admin123!");
        var updateAboutRoute = $"{ApiRoutes.About.Update}?newAbout=newAbout";

        // Act

        var aboutResponse = await HttpClient.PutAsync(updateAboutRoute, null);

        // Assert
        aboutResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateAbout_WhenUserIsNotAdmin_ReturnsForbidden()
    {
        // Arrange
        await this.SignupAndAuthenticateUserAsync("test", "testtest");
        var updateAboutRoute = $"{ApiRoutes.About.Update}?newAbout=newAbout";

        // Act
        var aboutResponse = await HttpClient.PutAsync(updateAboutRoute, null);

        // Assert
        aboutResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateAbout_WhenUserIsNotAuthenticated_ReturnsUnauthorized()
    {
        // Arrange
        var updateAboutRoute = $"{ApiRoutes.About.Update}?newAbout=newAbout";

        // Act
        var aboutResponse = await HttpClient.PutAsync(updateAboutRoute, null);

        // Assert
        aboutResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.Unauthorized);
    }

    public override Task DisposeAsync()
    {
        _mockedAboutServiceHttpClient.Dispose();
        _mockedAboutServiceFactory.Dispose();
        return base.DisposeAsync();
    }
}