using Lowsharp.Server.Data;

namespace Lowsharp.Server.Services;

internal sealed class CacheCleanupService : IHostedService, IDisposable
{
    private readonly ILogger<CacheCleanupService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private Timer? _timer = null;

    public CacheCleanupService(ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
    {
        _logger = loggerFactory.CreateLogger<CacheCleanupService>();
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"{nameof(CacheCleanupService)} starting");

        _timer = new Timer(callback: DoWork, state: null, dueTime: TimeSpan.Zero, period: TimeSpan.FromMinutes(25));

        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            JsonDbContextCache jsonDbContextCache = scope.ServiceProvider.GetRequiredService<JsonDbContextCache>();
            int removed = jsonDbContextCache.CleanOldEntries();
            _logger.LogInformation("Cleanup removed {count} cached items", removed);
        }
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"{nameof(CacheCleanupService)} is stopping");
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
