namespace GeoSpot.Application.Services.Interfaces;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    
    Task<T?> GetOrSetAsync<T>(string key, T value, TimeSpan ttl);
    
    Task SetAsync<T>(string key, T value, TimeSpan ttl);
}