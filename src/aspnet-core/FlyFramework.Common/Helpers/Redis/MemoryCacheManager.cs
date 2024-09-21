using Microsoft.Extensions.Caching.Memory;

namespace FlyFramework.Common.Helpers.Redis
{
    public class MemoryCacheManager : ICacheManager
    {
        // 内存缓存实例
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheManager(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public T GetCache<T>(string key)
        {
            return _memoryCache.Get<T>(key);
        }

        public async Task<T> GetCacheAsync<T>(string key)
        {
            return await Task.FromResult(GetCache<T>(key));
        }

        public void RemoveCache(string key)
        {
            _memoryCache.Remove(key);
        }

        public async Task RemoveCacheAsync(string key)
        {
            await Task.Run(() => RemoveCache(key));
        }

        public void SetCache<T>(string key, T value, DateTime? expireTime = null)
        {
            if (expireTime == null)
            {
                _memoryCache.Set(key, value);
            }
            else
            {
                DateTimeOffset dateTimeOffset = new DateTimeOffset(expireTime.Value, TimeSpan.Zero);  // UTC偏移量为0
                _memoryCache.Set(key, value, dateTimeOffset);
            }
        }

        public async Task SetCacheAsync<T>(string key, T value, DateTime? expireTime = null)
        {
            await Task.Run(() => SetCache(key, value, expireTime));
        }
    }
}
