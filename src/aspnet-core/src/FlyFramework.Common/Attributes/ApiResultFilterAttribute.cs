﻿using FlyFramework.ErrorExceptions;
using FlyFramework.Extentions;

using log4net;

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

namespace FlyFramework.Attributes
{
    public class ApiResultFilterAttribute : ActionFilterAttribute
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ApiResultFilterAttribute));
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
                var apiResponse = new ApiResponse<object>(
                   false,
                   context.Exception.Message + ": " + errorDetails,
                   null
                );
                log.Info(apiResponse.Message);
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

                    var apiResponse = new ApiResponse<object>(
                      false,
                      "错误或无效响应",
                      result.Value
                    );
                    SetContentResult(context, apiResponse, result.StatusCode ?? StatusCodes.Status500InternalServerError);
                }
                else
                {
                    var apiResponse = new ApiResponse<object>
                    (
                      true,
                      "请求成功",
                      result.Value
                    );
                    SetContentResult(context, apiResponse, StatusCodes.Status200OK);
                }
            }
        }

        private static void SetContentResult(ActionExecutedContext context, ApiResponse<object> response, int statusCode)
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
