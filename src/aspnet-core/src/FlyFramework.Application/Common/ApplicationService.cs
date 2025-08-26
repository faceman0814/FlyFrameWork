using FlyFramework.Cache;
using FlyFramework.Uow;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FlyFramework.Application
{
    /// <summary>
    /// 应用服务基类
    /// </summary>
    public abstract class ApplicationService
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// 工作单元管理器
        /// </summary>
        protected IUnitOfWorkManager UnitOfWorkManager { get; }

        /// <summary>
        /// 缓存服务
        /// </summary>
        protected ICacheService CacheService { get; }

        protected ApplicationService(
            ILogger logger,
            IUnitOfWorkManager unitOfWorkManager,
            ICacheService cacheService)
        {
            Logger = logger;
            UnitOfWorkManager = unitOfWorkManager;
            CacheService = cacheService;
        }

        /// <summary>
        /// 执行带缓存的操作
        /// </summary>
        protected async Task<T> ExecuteWithCacheAsync<T>(
            string cacheKey,
            Func<Task<T>> operation,
            TimeSpan? expiration = null)
        {
            // 尝试从缓存获取
            var cachedResult = await CacheService.GetAsync<T>(cacheKey);
            if (cachedResult != null)
            {
                Logger.LogDebug("从缓存获取数据，key: {CacheKey}", cacheKey);
                return cachedResult;
            }

            // 执行操作
            var result = await operation();

            // 缓存结果
            if (result != null)
            {
                await CacheService.SetAsync(cacheKey, result, expiration);
                Logger.LogDebug("缓存数据，key: {CacheKey}", cacheKey);
            }

            return result;
        }

        /// <summary>
        /// 执行带事务的操作
        /// </summary>
        protected async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation)
        {
            using var uow = UnitOfWorkManager.Begin();
            try
            {
                var result = await operation();
                await uow.SaveChangesAsync();
                return result;
            }
            catch
            {
                await uow.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// 执行带事务的操作（无返回值）
        /// </summary>
        protected async Task ExecuteInTransactionAsync(Func<Task> operation)
        {
            using var uow = UnitOfWorkManager.Begin();
            try
            {
                await operation();
                await uow.SaveChangesAsync();
            }
            catch
            {
                await uow.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// 清除相关缓存
        /// </summary>
        protected async Task ClearCacheAsync(params string[] cacheKeys)
        {
            foreach (var key in cacheKeys)
            {
                await CacheService.RemoveAsync(key);
                Logger.LogDebug("清除缓存，key: {CacheKey}", key);
            }
        }
    }
}