

using System.Runtime.Caching;

namespace LowSharp.Core;

public sealed class CachedLowerer : ILowerer
{
    private readonly ILowerer _lowerer;
    private readonly MemoryCache _memoryCache;

    public CachedLowerer(ILowerer lowerer)
    {
        _lowerer = lowerer;
        _memoryCache = new MemoryCache(nameof(CachedLowerer));
    }

    public Task<string?> CreateExport(LowerRequest request, CancellationToken cancellationToken)
        => _lowerer.CreateExport(request, cancellationToken);

    public IOrderedEnumerable<ComponentVersion> GetComponentVersions()
        => _lowerer.GetComponentVersions();

    public async Task<LowerResponse> ToLowerCodeAsync(LowerRequest request, CancellationToken cancellationToken)
    {
        var cacheItem = _memoryCache[request.ToString()] as LowerResponse;
        if (cacheItem != null)
        {
            return cacheItem;
        }

        CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
        cacheItemPolicy.SlidingExpiration = TimeSpan.FromMinutes(3);

        var result = await _lowerer.ToLowerCodeAsync(request, cancellationToken);

        _memoryCache.Set(request.ToString(), result, cacheItemPolicy);

        return result;
    }
}