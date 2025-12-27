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

    public async Task<T?> GetAsync<T>(string key)
    {
        return await Task.FromResult(_memoryCache.Get<T>(key));
    }

    public async Task<T?> GetOrSetAsync<T>(string key, T value, TimeSpan ttl)
    {
        return await _memoryCache.GetOrCreateAsync(key, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = ttl;
            return Task.FromResult(value);
        });
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan ttl)
    {
        await Task.FromResult(_memoryCache.Set(key, value, ttl));
    }
}