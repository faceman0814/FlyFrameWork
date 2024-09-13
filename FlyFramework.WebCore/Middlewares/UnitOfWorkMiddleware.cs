using FlyFramework.Common.Uow;

using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.WebCore.Middlewares
{
    public class UnitOfWorkMiddleware
    {
        private readonly RequestDelegate _next;

        public UnitOfWorkMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, IUnitOfWork unitOfWork)
        {
            try
            {
                // 调用下一个中间件
                await _next(httpContext);
                // 如果HTTP响应成功，则尝试提交事务
                if (httpContext.Response.StatusCode == 200)
                {
                    // 提交数据库事务
                    await unitOfWork.CommitAsync();
                }
                else
                {
                    // 如果在处理中发生了错误（例如业务逻辑错误导致的非200响应），则回滚事务
                    await unitOfWork.RollbackAsync();
                }
            }
            catch (Exception ex)
            {
                // 如果抛出异常，则回滚事务
                await unitOfWork.RollbackAsync();

                // 设置HTTP响应状态为500（服务器内部错误）
                httpContext.Response.StatusCode = 500;
                // 写入错误信息
                await httpContext.Response.WriteAsync($"An error occurred: {ex.Message}");

                // 重新抛出异常，可选，根据你是否想在日志中记录这些异常
                throw;
            }
        }
    }
}
