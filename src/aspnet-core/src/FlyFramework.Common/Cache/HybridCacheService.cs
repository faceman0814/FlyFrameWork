using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace FlyFramework.Cache
{
    /// <summary>
    /// 混合缓存服务实现（内存缓存 + 分布式缓存）
    /// </summary>
    public class HybridCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;

        public HybridCacheService(IMemoryCache memoryCache, IDistributedCache distributedCache)
        {
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            // 首先从内存缓存获取
            if (_memoryCache.TryGetValue(key, out T memValue))
            {
                return memValue;
            }

            // 从分布式缓存获取
            var distributedValue = await _distributedCache.GetStringAsync(key);
            if (!string.IsNullOrEmpty(distributedValue))
            {
                var value = JsonSerializer.Deserialize<T>(distributedValue);

                // 回写到内存缓存，设置较短的过期时间
                _memoryCache.Set(key, value, TimeSpan.FromMinutes(5));

                return value;
            }

            return default(T);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var options = new MemoryCacheEntryOptions();
            var distributedOptions = new DistributedCacheEntryOptions();

            if (expiration.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = expiration;
                distributedOptions.AbsoluteExpirationRelativeToNow = expiration;
            }
            else
            {
                // 默认过期时间
                options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                distributedOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
            }

            // 设置内存缓存
            _memoryCache.Set(key, value, options);

            // 设置分布式缓存
            var serializedValue = JsonSerializer.Serialize(value);
            await _distributedCache.SetStringAsync(key, serializedValue, distributedOptions);
        }

        public async Task RemoveAsync(string key)
        {
            _memoryCache.Remove(key);
            await _distributedCache.RemoveAsync(key);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            if (_memoryCache.TryGetValue(key, out _))
            {
                return true;
            }

            var distributedValue = await _distributedCache.GetStringAsync(key);
            return !string.IsNullOrEmpty(distributedValue);
        }

        public async Task RemoveByPatternAsync(string pattern)
        {
            // 内存缓存的模式删除需要额外实现
            // 分布式缓存的模式删除依赖于具体实现（Redis支持）
            await Task.CompletedTask;
        }
    }
}