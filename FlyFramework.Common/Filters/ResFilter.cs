using FlyFramework.Common.Extentions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Common.Filters
{
    public class ResFilter : ActionFilterAttribute
    {
        /// <summary>
        /// 在操作方法执行后被调用
        /// </summary>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            // 检查是否存在异常, 如果有，则不处理数据
            if (context.Exception != null)
            {
                return;
            }

            // 获取从 action 返回的结果对象
            var result = context.Result as ObjectResult;

            if (result == null)
            {
                // Assume success if no result is provided
                context.Result = new ObjectResult(new ApiResponse
                {
                    Success = true,
                    Message = "请求成功",
                    Data = null
                })
                {
                    StatusCode = StatusCodes.Status200OK
                };
                goto end;
            }
            // 根据业务需求，可以对 result 进行检测和修改
            if (result?.Value == null || result.StatusCode < 200 || result.StatusCode >= 300)
            {
                // 可以处理错误或者不合适的返回状态码
                context.Result = new ObjectResult(new ApiResponse
                {
                    Success = false,
                    Message = "Error or invalid response",
                    Data = result?.Value
                })
                {
                    StatusCode = result?.StatusCode ?? 500
                };
            }
            else
            {
                // 包装原始结果为统一的返回格式
                context.Result = new ObjectResult(new
                {
                    success = true,
                    message = "Success",
                    data = result.Value
                })
                {
                    StatusCode = result.StatusCode
                };
            }
        end:
            base.OnActionExecuted(context);
        }
    }
}
