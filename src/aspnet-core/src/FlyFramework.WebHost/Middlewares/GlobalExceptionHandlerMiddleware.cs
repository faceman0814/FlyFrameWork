using FlyFramework.ErrorExceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace FlyFramework.Middlewares
{
    /// <summary>
    /// 全局异常处理中间件
    /// </summary>
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "发生未处理的异常: {Message}", exception.Message);

            context.Response.ContentType = "application/json";

            var response = new
            {
                success = false,
                error = new
                {
                    message = GetErrorMessage(exception),
                    details = GetErrorDetails(exception),
                    code = GetErrorCode(exception)
                },
                targetUrl = (string)null,
                unAuthorizedRequest = false
            };

            context.Response.StatusCode = GetStatusCode(exception);

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }

        private static string GetErrorMessage(Exception exception)
        {
            return exception switch
            {
                UserFriendlyException userFriendlyException => userFriendlyException.Message,
                UnauthorizedAccessException => "未授权访问",
                ArgumentException => "参数错误",
                InvalidOperationException => "操作无效",
                TimeoutException => "请求超时",
                _ => "系统内部错误，请联系管理员"
            };
        }

        private static string GetErrorDetails(Exception exception)
        {
            return exception switch
            {
                UserFriendlyException userFriendlyException => userFriendlyException.Details ?? string.Empty,
                _ => exception.Message
            };
        }

        private static int GetErrorCode(Exception exception)
        {
            return exception switch
            {
                UserFriendlyException userFriendlyException => userFriendlyException.Code,
                ArgumentException => 400,
                UnauthorizedAccessException => 401,
                InvalidOperationException => 422,
                TimeoutException => 408,
                _ => 500
            };
        }

        private static int GetStatusCode(Exception exception)
        {
            return exception switch
            {
                UserFriendlyException => (int)HttpStatusCode.BadRequest,
                ArgumentException => (int)HttpStatusCode.BadRequest,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                InvalidOperationException => (int)HttpStatusCode.UnprocessableEntity,
                TimeoutException => (int)HttpStatusCode.RequestTimeout,
                _ => (int)HttpStatusCode.InternalServerError
            };
        }
    }
}