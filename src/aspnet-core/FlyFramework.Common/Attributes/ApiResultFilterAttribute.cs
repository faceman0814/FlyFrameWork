using FlyFramework.Common.ErrorExceptions;
using FlyFramework.Common.Extentions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Common.Attributes
{
    public class ApiResultFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 在操作方法执行后被调用
        /// </summary>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                if (!HandleException(context))
                {
                    return;
                }
            }
            else
            {
                ProcessResult(context);
            }

            base.OnActionExecuted(context);
        }

        private bool HandleException(ActionExecutedContext context)
        {
            if (!context.ExceptionHandled && context.Exception is UserFriendlyException userFriendlyException)
            {
                var errorDetails = userFriendlyException.Details;
                var apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = context.Exception.Message + ": " + errorDetails,
                    Data = null
                };

                SetContentResult(context, apiResponse, StatusCodes.Status500InternalServerError);
                context.ExceptionHandled = true;
                return true;
            }
            return false;
        }

        private void ProcessResult(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult result)
            {
                if (result.StatusCode < 200 || result.StatusCode >= 300)
                {
                    var apiResponse = new ApiResponse
                    {
                        Success = false,
                        Message = "错误或无效响应",
                        Data = result.Value
                    };

                    SetContentResult(context, apiResponse, result.StatusCode ?? StatusCodes.Status500InternalServerError);
                }
                else
                {
                    var apiResponse = new ApiResponse
                    {
                        Success = true,
                        Message = "请求成功",
                        Data = result.Value // 改为使用原始数据，避免数据丢失
                    };

                    SetContentResult(context, apiResponse, StatusCodes.Status200OK);
                }
            }
        }

        private static void SetContentResult(ActionExecutedContext context, ApiResponse response, int statusCode)
        {
            context.Result = new ContentResult
            {
                StatusCode = statusCode,
                ContentType = "application/json;charset=utf-8",
                Content = JsonConvert.SerializeObject(response)
            };
        }
    }
}
