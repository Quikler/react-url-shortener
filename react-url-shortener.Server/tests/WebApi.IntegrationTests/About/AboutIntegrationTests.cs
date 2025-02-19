using System.Net.Http.Json;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shouldly;
using WebApi.Contracts;
using WebApi.Contracts.V1.Responses;
using WebApi.DTOs;
using WebApi.IntegrationTests.Abstractions;
using WebApi.Services.About;

namespace WebApi.IntegrationTests.About;

public class AboutIntegrationTests(IntegrationTestWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
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
        var mockAboutService = new Mock<IAboutService>();
        mockAboutService
            .Setup(s => s.GetAboutAsync())
            .ReturnsAsync(FailureDto.BadRequest("Cannot update about."));

        var modifiedFactory = Factory.WithWebHostBuilder(builder =>
        {
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

        using var scope = modifiedFactory.Services.CreateScope();
        var aboutService = scope.ServiceProvider.GetRequiredService<IAboutService>();

        var httpClient = modifiedFactory.CreateClient();

        // Act
        var aboutResponse = await httpClient.GetAsync(ApiRoutes.About.Get);
        aboutResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);

        // Assert
        var failureResponse = await aboutResponse.Content.ReadFromJsonAsync<FailureResponse>();
        failureResponse.ShouldNotBeNull();
        failureResponse.Errors.ShouldContain("Cannot update about.");
    }
}