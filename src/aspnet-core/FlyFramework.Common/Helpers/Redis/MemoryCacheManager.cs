using Microsoft.Extensions.Caching.Memory;

using StackExchange.Redis;

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

        public async Task<Dictionary<string, T>> GetHashCache<T>(string key)
        {
            // 尝试从内存缓存中获取数据。
            if (_memoryCache.TryGetValue(key, out Dictionary<string, T> dictionary))
            {
                // 如果找到了数据，直接返回。
                return dictionary;
            }
            //else
            //{
            //    dictionary = new Dictionary<string, T>(); // 创建一个新的字典，这会在实际场景中替换为真实的数据加载逻辑

            //    // 例如：_memoryCache.Set(key, dictionary, TimeSpan.FromMinutes(30)); //设置过期时间等。

            //    return dictionary;
            //}
            return null;
        }

        public async Task<T> GetHashFieldCache<T>(string key, string fieldKey)
        {
            // 确保字典已经被加载到缓存中
            var hashCache = await GetHashCache<T>(key);

            if (hashCache != null && hashCache.TryGetValue(fieldKey, out T value))
            {
                // 如果找到了对应字段的值，返回这个值
                return value;
            }
            else
            {
                // 这里处理找不到字段的情况，可以返回默认值或者抛出异常
                return default(T);  // 或 throw new KeyNotFoundException($"Field key {fieldKey} not found in cache for key {key}.");
            }
        }

        public Task<Dictionary<string, T>> GetHashFieldCache<T>(string key, Dictionary<string, T> dict)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> GetHashToListCache<T>(string key)
        {
            throw new NotImplementedException();
        }

        public Task<long> GetIncr(string key)
        {
            throw new NotImplementedException();
        }

        public Task<long> GetIncr(string key, TimeSpan expTimeSpan)
        {
            throw new NotImplementedException();
        }

        public ISubscriber GetSubscriber()
        {
            throw new NotImplementedException();
        }

        public Task<long> ListLeftPushAsync(string redisKey, string redisValue)
        {
            throw new NotImplementedException();
        }

        public Task<long> ListRightPushAsync(string redisKey, string redisValue)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveCache(string key)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveHashFieldCache(string key, string fieldKey)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, bool>> RemoveHashFieldCache(string key, Dictionary<string, bool> dict)
        {
            throw new NotImplementedException();
        }

        public Task RemoveKeysLeftLike(string key)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetCache<T>(string key, T value, DateTime? expireTime = null)
        {
            throw new NotImplementedException();
        }

        public Task<int> SetHashFieldCache<T>(string key, string fieldKey, T fieldValue)
        {
            throw new NotImplementedException();
        }

        public Task<int> SetHashFieldCache<T>(string key, Dictionary<string, T> dict)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SortedSetAddAsync(string redisKey, string redisValue, DateTime time)
        {
            throw new NotImplementedException();
        }

        Task<T> ICacheManager.GetCache<T>(string key)
        {
            throw new NotImplementedException();
        }
    }
}
