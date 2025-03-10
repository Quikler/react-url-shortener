using AutoFixture;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using WebApi.Repositories.Url;

namespace WebApi.UnitTests.Repositories.Url;

public class BaseUrlRepositoryTests : IAsyncLifetime
{
    public AppDbContext DbContext { get; }
    public UrlRepository UrlRepository { get; }
    public IMemoryCache MemoryCache { get; }
    public Fixture Fixture { get; }

    private readonly string _connectionString;

    public BaseUrlRepositoryTests()
    {
        Fixture = new Fixture();
        Fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _connectionString = $"Data Source={Guid.NewGuid()}";

        var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connectionString)
            .Options;

        DbContext = new AppDbContext(dbContextOptions);

        MemoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
        UrlRepository = new UrlRepository(DbContext, MemoryCache);
    }

    public async Task InitializeAsync()
    {
        var pendingMigrations = await DbContext.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            DbContext.Database.Migrate();
        }
    }

    public async Task DisposeAsync()
    {
        await DbContext.Database.EnsureDeletedAsync();
    }
}