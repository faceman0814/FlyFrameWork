using FlyFramework.Attributes;
using FlyFramework.Uow;

using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace FlyFramework.Filters
{

    public class UnitOfWorkFilter : IAsyncActionFilter, IOrderedFilter
    {

        private readonly ILogger<UnitOfWorkFilter> _logger;
        public UnitOfWorkFilter(ILogger<UnitOfWorkFilter> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// 过滤器排序
        /// </summary>
        internal const int FilterOrder = 999;

        /// <summary>
        /// 排序属性
        /// </summary>
        public int Order => FilterOrder;

        /// <summary>
        /// 拦截请求
        /// </summary>
        /// <param name="context">动作方法上下文</param>
        /// <param name="next">中间件委托</param>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // 获取动作方法描述器
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            var method = actionDescriptor?.MethodInfo;

            if (method == null)
            {
                await next();
                return;
            }

            // 获取请求上下文
            var httpContext = context.HttpContext;

            // 如果没有定义工作单元过滤器，则跳过
            if (method.IsDefined(typeof(DisabledUnitOfWorkAttribute), true))
            {
                await next();
                return;
            }

            // 解析工作单元服务
            var unitOfWorks = httpContext.RequestServices.GetServices<IUnitOfWork>();
            var activeUnitOfWorks = new List<IUnitOfWork>();

            try
            {
                // 开启事务
                foreach (var unitOfWork in unitOfWorks)
                {
                    await unitOfWork.BeginAsync();
                    activeUnitOfWorks.Add(unitOfWork);
                }

                var result = await next();

                // 只有在成功执行时才提交事务
                if (result.Exception == null)
                {
                    foreach (var unitOfWork in activeUnitOfWorks)
                    {
                        await unitOfWork.SaveChangesAsync();
                    }
                }
                else
                {
                    // 如果有异常，回滚事务
                    await RollbackTransactions(activeUnitOfWorks);
                    _logger.LogWarning("由于执行过程中出现异常，已回滚所有事务");
                }
            }
            catch (Exception ex)
            {
                // 回滚所有事务
                await RollbackTransactions(activeUnitOfWorks);
                _logger.LogError(ex, "工作单元过滤器执行过程中发生异常: {Message}", ex.Message);
                throw;
            }
        }

        private async Task RollbackTransactions(List<IUnitOfWork> unitOfWorks)
        {
            foreach (var unitOfWork in unitOfWorks)
            {
                try
                {
                    await unitOfWork.RollbackAsync();
                }
                catch (Exception rollbackEx)
                {
                    _logger.LogError(rollbackEx, "回滚事务时发生异常: {Message}", rollbackEx.Message);
                }
            }
        }
    }
}
