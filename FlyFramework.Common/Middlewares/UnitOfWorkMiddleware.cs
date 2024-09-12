using FlyFramework.Common.Uow;

using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Common.Middlewares
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
                await _next(httpContext);
                await unitOfWork.CommitAsync();
            }
            catch (Exception)
            {
                // 处理异常，可能需要回滚
                throw; // 确保异常能够继续传递
            }
        }
    }
}
