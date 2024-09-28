using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

using ServiceStack.Redis;

using System;
using System.Threading.Tasks;
namespace FlyFramework.Utilities.Redis
{
    public class RedisCacheManager : ICacheManager
    {
        private readonly IRedisClient _redisClient;
        private readonly IConfiguration _configuration;

        public RedisCacheManager(IRedisClient redisClient, IConfiguration configuration)
        {
            _redisClient = redisClient;
            _configuration = configuration;

        }

        public async void SetCache<T>(string key, T value, DateTime? expireTime = null)
        {
            try
            {
                var jsonOption = new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                var strValue = JsonConvert.SerializeObject(value, jsonOption);
                if (string.IsNullOrEmpty(strValue))
                {
                    throw new ArgumentException("Value cannot be null or empty.");
                }
                if (expireTime == null)
                {
                    _redisClient.Set(InitKey(key), strValue);

                }
                else
                {
                    _redisClient.Set(InitKey(key), strValue, expireTime.Value);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        public T GetCache<T>(string key)
        {
            var t = default(T);
            //try
            //{
            var value = _redisClient.Get<string>(InitKey(key));
            if (string.IsNullOrEmpty(value)) return t;
            t = JsonConvert.DeserializeObject<T>(value);
            //}
            //catch (Exception ex)
            //{
            //    // ignored
            //}

            return t;
        }

        public void RemoveCache(string key)
        {
            _redisClient.Delete(InitKey(key));
        }

        private string InitKey(string key)
        {
            return $"{_configuration["Redis:preName"]}{key}";
        }

        public async Task SetCacheAsync<T>(string key, T value, DateTime? expireTime = null)
        {
            await Task.Run(() => SetCache(key, value, expireTime));
        }

        public async Task<T> GetCacheAsync<T>(string key)
        {
            return await Task.FromResult(GetCache<T>(key));
        }

        public async Task RemoveCacheAsync(string key)
        {
            await Task.Run(() => RemoveCache(key));
        }
    }
}
