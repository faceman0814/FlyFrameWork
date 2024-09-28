using System;
using System.Threading.Tasks;
namespace FlyFramework.Utilities.Redis
{
    public interface ICacheManager
    {
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireTime"></param>
        /// <returns></returns>
        Task SetCacheAsync<T>(string key, T value, DateTime? expireTime = null);
        void SetCache<T>(string key, T value, DateTime? expireTime = null);

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> GetCacheAsync<T>(string key);
        T GetCache<T>(string key);

        /// <summary>
        /// 根据键精准删除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task RemoveCacheAsync(string key);
        void RemoveCache(string key);
    }
}
