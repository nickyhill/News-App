using Microsoft.Extensions.Caching.Memory;


namespace CyberNewsApp.Services
{
    public class CacheService
    {
        private readonly IMemoryCache _memoryCache;

        public CacheService()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
        }

        public T? GetFromMemory<T>(string key)
        {
            return _memoryCache.TryGetValue(key, out T value) ? value : default;
        }

        public void SetInMemory<T>(string key, T value, TimeSpan expiration)
        {
            _memoryCache.Set(key, value, expiration);
        }
    }
}
