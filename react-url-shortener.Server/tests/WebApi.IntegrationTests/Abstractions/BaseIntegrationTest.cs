using DAL;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi.IntegrationTests.Abstractions;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebApplicationFactory>
{
    protected HttpClient HttpClient { get; init; }
    protected AppDbContext DbContext { get; init; }

    protected BaseIntegrationTest(IntegrationTestWebApplicationFactory factory)
    {
        HttpClient = factory.CreateClient();

        var scope = factory.Services.CreateScope();
        DbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }
}