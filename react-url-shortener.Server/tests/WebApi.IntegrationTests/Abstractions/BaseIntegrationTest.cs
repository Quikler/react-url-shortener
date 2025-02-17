using DAL;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi.IntegrationTests.Abstractions;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebApplicationFactory>, IAsyncLifetime
{
    protected HttpClient HttpClient { get; private set; } = null!;
    protected AppDbContext DbContext { get; private set; } = null!;
    private IServiceScope? _scope;

    protected BaseIntegrationTest(IntegrationTestWebApplicationFactory factory)
    {
        // HttpClient = factory.CreateClient();

        // _scope = factory.Services.CreateScope();
        // DbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    public async Task InitializeAsync()
    {
        var newFactory = new IntegrationTestWebApplicationFactory();
        HttpClient = newFactory.CreateClient();
        _scope = newFactory.Services.CreateScope();
        DbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Ensure the database is created
        await DbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await DbContext.Database.EnsureDeletedAsync();
        _scope?.Dispose();
        HttpClient?.Dispose();
    }
}