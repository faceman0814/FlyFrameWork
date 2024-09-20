﻿using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

using StackExchange.Redis;

namespace FlyFramework.Common.Helpers.Redis
{
    public class CacheManager : ICacheManager
    {
        private readonly IDatabase _database;
        private readonly ConnectionMultiplexer _connectionMultiplexer;
        private ConnectionMultiplexer connection;
        private readonly IConfiguration _configuration;

        public CacheManager(IConfiguration configuration)
        {
            //把appsetting.json中配置的Redis连接配置注入进来，连接Redis
            _configuration = configuration;
            string redisHost = _configuration["Redis:host"];
            int redisPort = int.Parse(_configuration["Redis:port"]);
            var configurationOptions = new ConfigurationOptions
            {
                EndPoints =
            {
                { redisHost, redisPort }
            },
                KeepAlive = 180,
                Password = _configuration["Redis:password"],
                AllowAdmin = true
            };
            _connectionMultiplexer = ConnectionMultiplexer.Connect(configurationOptions);
            _database = _connectionMultiplexer.GetDatabase();

        }




        public async Task<bool> SetCache<T>(string key, T value, DateTime? expireTime = null)
        {
            try
            {
                var jsonOption = new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                var strValue = JsonConvert.SerializeObject(value, jsonOption);
                if (string.IsNullOrEmpty(strValue))
                    return false;
                if (expireTime == null)
                    return await _database.StringSetAsync(InitKey(key), strValue);

                return await _database.StringSetAsync(InitKey(key), strValue, expireTime.Value - DateTime.Now);
            }
            catch (Exception ex) { }

            return false;
        }

        public async Task<T> GetCache<T>(string key)
        {
            var t = default(T);
            try
            {
                var value = await _database.StringGetAsync(InitKey(key));
                if (string.IsNullOrEmpty(value)) return t;
                t = JsonConvert.DeserializeObject<T>(value);
            }
            catch (Exception ex)
            {
                // ignored
            }

            return t;
        }

        public async Task<bool> RemoveCache(string key)
        {
            return await _database.KeyDeleteAsync(InitKey(key));
        }

        public async Task RemoveKeysLeftLike(string key)
        {
            var redisResult = await _database.ScriptEvaluateAsync(LuaScript.Prepare(
                //Redis的keys模糊查询：
                " local res = redis.call('KEYS', @keywords) " +
                " return res "), new { @keywords = $"{InitKey(key)}*" });
            if (!redisResult.IsNull)
                await _database.KeyDeleteAsync((RedisKey[])redisResult); //删除一组key
        }

        public async Task<long> GetIncr(string key)
        {
            try
            {
                return await _database.StringIncrementAsync(InitKey(key));
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public async Task<long> GetIncr(string key, TimeSpan expTimeSpan)
        {
            try
            {
                var qty = await _database.StringIncrementAsync(InitKey(key));
                if (qty == 1)
                {
                    //设置过期时间
                    await _database.KeyExpireAsync(key, expTimeSpan);
                }

                return qty;
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public async Task<int> SetHashFieldCache<T>(string key, string fieldKey, T fieldValue)
        {
            return await SetHashFieldCache<T>(InitKey(key), new Dictionary<string, T> { { fieldKey, fieldValue } });
        }

        public async Task<int> SetHashFieldCache<T>(string key, Dictionary<string, T> dict)
        {
            var count = 0;
            var jsonOption = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            foreach (var fieldKey in dict.Keys)
            {
                var fieldValue = JsonConvert.SerializeObject(dict[fieldKey], jsonOption);
                count += await _database.HashSetAsync(InitKey(key), fieldKey, fieldValue) ? 1 : 0;
            }
            return count;
        }

        public async Task<T> GetHashFieldCache<T>(string key, string fieldKey)
        {
            var dict = await GetHashFieldCache<T>(InitKey(key), new Dictionary<string, T> { { fieldKey, default(T) } });
            return dict[fieldKey];
        }

        public async Task<Dictionary<string, T>> GetHashFieldCache<T>(string key, Dictionary<string, T> dict)
        {
            foreach (string fieldKey in dict.Keys)
            {
                string fieldValue = await _database.HashGetAsync(InitKey(key), fieldKey);
                dict[fieldKey] = JsonConvert.DeserializeObject<T>(fieldValue);
            }
            return dict;
        }

        public async Task<Dictionary<string, T>> GetHashCache<T>(string key)
        {
            var dict = new Dictionary<string, T>();
            var hashFields = await _database.HashGetAllAsync(InitKey(key));
            foreach (var field in hashFields)
            {
                dict[field.Name] = JsonConvert.DeserializeObject<T>(field.Value);
            }
            return dict;
        }

        public async Task<List<T>> GetHashToListCache<T>(string key)
        {
            var list = new List<T>();
            var hashFields = await _database.HashGetAllAsync(InitKey(key));
            foreach (var field in hashFields)
            {
                list.Add(JsonConvert.DeserializeObject<T>(field.Value));
            }
            return list;
        }

        public async Task<bool> RemoveHashFieldCache(string key, string fieldKey)
        {
            var dict = new Dictionary<string, bool> { { fieldKey, false } };
            dict = await RemoveHashFieldCache(InitKey(key), dict);
            return dict[fieldKey];
        }

        public async Task<Dictionary<string, bool>> RemoveHashFieldCache(string key, Dictionary<string, bool> dict)
        {
            foreach (var fieldKey in dict.Keys)
            {
                dict[fieldKey] = await _database.HashDeleteAsync(InitKey(key), fieldKey);
            }
            return dict;
        }



        #region list

        public async Task<long> ListLeftPushAsync(string redisKey, string redisValue)
        {
            return await _database.ListLeftPushAsync(redisKey, redisValue);
        }

        public async Task<long> ListRightPushAsync(string redisKey, string redisValue)
        {
            return await _database.ListRightPushAsync(redisKey, redisValue);
        }

        #endregion

        #region 有序集合(Sorted Set)

        public async Task<bool> SortedSetAddAsync(string redisKey, string redisValue, DateTime time)
        {
            //var sort = DateHelper.ToUnixTimestampBySeconds(time);
            //return await _database.SortedSetAddAsync(redisKey, redisValue, sort);
            return false;
        }

        #endregion


        private string InitKey(string key)
        {
            //return $"{_webApplicationBuilder.Configuration["Redis:preName"]}{key}";
            return $"{_configuration["Redis:preName"]}{key}";
        }


        public ISubscriber GetSubscriber()
        {
            return _connectionMultiplexer.GetSubscriber();
        }

    }
}
