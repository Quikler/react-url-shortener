using DAL;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Jobs;

public class RefreshTokenRemoverHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public RefreshTokenRemoverHostedService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await DoWorkAsync();
            await Task.Delay(TimeSpan.FromMinutes(1));
        }
    }

    private async Task DoWorkAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<RefreshTokenRemoverHostedService>>();

        try
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            int rows = await dbContext.RefreshTokens
                .Where(r => r.ExpiryDate < DateTime.UtcNow)
                .ExecuteDeleteAsync();

            logger.LogInformation("{DT}: {ROWS} expired refresh tokens has been removed", DateTime.UtcNow.ToLongTimeString(), rows);
        }
        catch (System.Exception e)
        {
            logger.LogError(e, "Error occured while removing expired refresh tokens.");
        }
    }
}
