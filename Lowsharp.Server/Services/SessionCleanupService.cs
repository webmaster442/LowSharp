using Lowsharp.Server.Interactive;

namespace Lowsharp.Server.Services;

internal sealed class SessionCleanupService : IHostedService, IDisposable
{
    private readonly ILogger<CacheCleanupService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private Timer? _timer = null;

    public SessionCleanupService(ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
    {
        _logger = loggerFactory.CreateLogger<CacheCleanupService>();
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"{nameof(SessionCleanupService)} starting");

        _timer = new Timer(callback: DoWork, state: null, dueTime: TimeSpan.Zero, period: TimeSpan.FromMinutes(5));

        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            TimeProvider timeProvider = scope.ServiceProvider.GetRequiredService<TimeProvider>();
            SessionManager sessions = scope.ServiceProvider.GetRequiredService<SessionManager>();
            
            var toRemove = sessions
                .Where(s => s.lastaccessUtc < (timeProvider.GetUtcNow().AddMinutes(-5)))
                .Select(s => s.sessionId)
                .ToList();

            int count = 0;
            foreach (var sessionId in toRemove)
            {
                sessions.Remove(sessionId);
                ++count;
            }
            
            _logger.LogInformation("Cleanup removed {count} inactive sessions", count);
        }
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"{nameof(SessionCleanupService)} is stopping");
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}