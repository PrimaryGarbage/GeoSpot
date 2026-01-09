using GeoSpot.Application.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace GeoSpot.Application.Services;

[ExcludeFromCodeCoverage]
internal sealed class InMemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;

    public InMemoryCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        return Task.FromResult(_memoryCache.Get<T>(key));
    }

    public async Task<T?> GetOrSetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default)
    {
        return await _memoryCache.GetOrCreateAsync(key, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = ttl;
            return Task.FromResult(value);
        });
    }

    public Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default)
    {
        return Task.FromResult(_memoryCache.Set(key, value, ttl));
    }

    public Task RemoveAsync(string key, CancellationToken ct = default)
    {
        _memoryCache.Remove(key);
        return Task.CompletedTask;
    }
}