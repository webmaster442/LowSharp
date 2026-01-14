using Microsoft.Extensions.Caching.Memory;

namespace Lowsharp.Server.Infrastructure;

internal sealed class RequestCache
{
    private readonly ILogger<RequestCache> _logger;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheValidity = TimeSpan.FromMinutes(10);

    public RequestCache(IMemoryCache cache, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<RequestCache>();
        _cache = cache;
    }

    public async ValueTask<TResult> GetOrCreateAsync<TResult, TRequest>(TRequest request, Func<TRequest, Task<TResult>> factory)
        where TRequest : notnull, IEquatable<TRequest>
        where TResult : notnull
    {
        if (_cache.TryGetValue<TResult>(request, out TResult? cachedResult)
            && cachedResult != null)
        {
            _logger.LogInformation("Cache hit (memory) for key {Key}", request);
            return cachedResult;
        }

        _logger.LogInformation("Cache miss for key {Key}", request);
        TResult result = await factory(request);

        _cache.CreateEntry(request)
            .SetSlidingExpiration(_cacheValidity)
            .SetValue(result);

        return result;
    }

    public async ValueTask InvalidateAsync()
    {
        _logger.LogInformation("Invalidating entire cache");
        if (_cache is MemoryCache memCache)
        {
            memCache.Clear();
        }
    }
}
