using System.Security.Cryptography;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Lowsharp.Server.Data;

internal sealed class JsonDbContextCache
{
    private readonly ServerDbContext _dbContext;
    private readonly TimeSpan _memoryCacheDuration;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<JsonDbContextCache> _logger;

    public JsonDbContextCache(ServerDbContext dbContext,
                              TimeSpan memoryCacheDuration,
                              IMemoryCache memoryCache,
                              ILoggerFactory loggerFactory)
    {
        _dbContext = dbContext;
        _memoryCacheDuration = memoryCacheDuration;
        _memoryCache = memoryCache;
        _logger = loggerFactory.CreateLogger<JsonDbContextCache>();
    }

    private static string CreateKey<TRequest>(TRequest request)
    {
        string json = JsonSerializer.Serialize(request, JsonSerializerOptions.Web);
        byte[] bytes = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(json));
        return Convert.ToBase64String(bytes);
    }

    public async ValueTask<TResult> TryGetOrCreate<TResult, TRequest>(TRequest request, Func<TRequest, Task<TResult>> creator)
    {
        string key = CreateKey(request);

        if (_memoryCache.TryGetValue(key, out TResult? cachedResult)
            && cachedResult != null)
        {
            _logger.LogInformation("Cache hit (memory) for key {Key}", key);
            return cachedResult;
        }

        var dbResult = await _dbContext.CacheItems
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.Id == key);

        if (dbResult != null)
        {
            TResult? deserialized = JsonSerializer.Deserialize<TResult>(dbResult.Result, JsonSerializerOptions.Web);
            if (deserialized != null)
            {
                _logger.LogInformation("Cache hit (database) for key {Key}", key);
                _memoryCache.CreateEntry(key).SetSlidingExpiration(_memoryCacheDuration).SetValue(deserialized);
                return deserialized;
            }
        }

        _logger.LogInformation("Cache miss for key {Key}", key);

        TResult result = await creator(request);
        string serialized = JsonSerializer.Serialize(result, JsonSerializerOptions.Web);

        var cacheItem = new JsonCacheItem
        {
            Id = key,
            Result = serialized,
            CreateDate = DateTime.UtcNow
        };

        _dbContext.CacheItems.Add(cacheItem);
        await _dbContext.SaveChangesAsync();

        _memoryCache.CreateEntry(key).SetSlidingExpiration(_memoryCacheDuration).SetValue(result);

        return result;
    }

    public int CleanOldEntries()
    {
        var targetDate = DateTime.UtcNow - TimeSpan.FromDays(30);

         return _dbContext.CacheItems
            .Where(c => c.CreateDate < targetDate)
            .ExecuteDelete();
    }
}
