using DAL;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi.IntegrationTests.Abstractions;

public abstract class BaseIntegrationTest(IntegrationTestWebApplicationFactory factory) : IClassFixture<IntegrationTestWebApplicationFactory>, IAsyncLifetime
{
    private readonly IntegrationTestWebApplicationFactory _factory = factory;

    public HttpClient HttpClient { get; private set; } = null!;
    public AppDbContext DbContext { get; private set; } = null!;
    private IServiceScope? _scope;

    public async Task InitializeAsync()
    {
        var newFactory = new IntegrationTestWebApplicationFactory();
        // Change default http://localhost to HTTPS version TO PREVENT REDIRECTION from HTTP to HTTPS
        newFactory.ClientOptions.BaseAddress = new Uri("https://localhost");
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