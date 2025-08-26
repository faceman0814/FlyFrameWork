using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace FlyFramework.Filters
{
    /// <summary>
    /// API 结果包装过滤器
    /// </summary>
    public class ApiResultWrapperFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var executedContext = await next();

            // 如果有异常，不处理（由异常处理中间件处理）
            if (executedContext.Exception != null)
            {
                return;
            }

            // 如果已经是包装过的结果，不再包装
            if (executedContext.Result is ObjectResult objectResult)
            {
                var value = objectResult.Value;

                // 检查是否已经是统一格式
                if (IsWrappedResult(value))
                {
                    return;
                }

                // 包装结果
                var wrappedResult = new
                {
                    success = true,
                    result = value,
                    error = (object)null,
                    targetUrl = (string)null,
                    unAuthorizedRequest = false
                };

                executedContext.Result = new ObjectResult(wrappedResult)
                {
                    StatusCode = objectResult.StatusCode
                };
            }
            else if (executedContext.Result is EmptyResult)
            {
                // 空结果也包装
                var wrappedResult = new
                {
                    success = true,
                    result = (object)null,
                    error = (object)null,
                    targetUrl = (string)null,
                    unAuthorizedRequest = false
                };

                executedContext.Result = new ObjectResult(wrappedResult);
            }
        }

        private static bool IsWrappedResult(object value)
        {
            if (value == null) return false;

            var type = value.GetType();
            return type.GetProperty("success") != null &&
                   type.GetProperty("result") != null;
        }
    }
}