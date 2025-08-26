using System;
using System.Threading.Tasks;

namespace FlyFramework.Cache
{
    /// <summary>
    /// 缓存服务接口
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// 获取缓存值
        /// </summary>
        Task<T> GetAsync<T>(string key);

        /// <summary>
        /// 设置缓存值
        /// </summary>
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);

        /// <summary>
        /// 删除缓存
        /// </summary>
        Task RemoveAsync(string key);

        /// <summary>
        /// 检查缓存是否存在
        /// </summary>
        Task<bool> ExistsAsync(string key);

        /// <summary>
        /// 批量删除缓存
        /// </summary>
        Task RemoveByPatternAsync(string pattern);
    }
}