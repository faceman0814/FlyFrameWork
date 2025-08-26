using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FlyFramework.Middlewares
{
    /// <summary>
    /// 请求验证中间件
    /// </summary>
    public class RequestValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestValidationMiddleware> _logger;
        private readonly RequestValidationOptions _options;

        public RequestValidationMiddleware(
            RequestDelegate next,
            ILogger<RequestValidationMiddleware> logger,
            IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _options = new RequestValidationOptions();
            configuration.GetSection("RequestValidation").Bind(_options);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 检查请求速率限制
            if (await IsRateLimited(context))
            {
                context.Response.StatusCode = 429; // Too Many Requests
                await context.Response.WriteAsync("请求过于频繁，请稍后再试");
                return;
            }

            // 检查请求大小
            if (IsRequestTooLarge(context))
            {
                context.Response.StatusCode = 413; // Payload Too Large
                await context.Response.WriteAsync("请求体过大");
                return;
            }

            // 检查危险字符
            if (ContainsDangerousContent(context))
            {
                _logger.LogWarning("检测到危险请求内容，IP: {IP}, Path: {Path}",
                    GetClientIp(context), context.Request.Path);
                context.Response.StatusCode = 400; // Bad Request
                await context.Response.WriteAsync("请求包含非法内容");
                return;
            }

            await _next(context);
        }

        private async Task<bool> IsRateLimited(HttpContext context)
        {
            if (!_options.EnableRateLimit)
                return false;

            var clientIp = GetClientIp(context);
            // 这里应该实现基于Redis或内存的速率限制逻辑
            // 简单实现可以使用内存缓存存储请求次数
            return false;
        }

        private bool IsRequestTooLarge(HttpContext context)
        {
            if (context.Request.ContentLength.HasValue)
            {
                return context.Request.ContentLength.Value > _options.MaxRequestSize;
            }
            return false;
        }

        private bool ContainsDangerousContent(HttpContext context)
        {
            var dangerousPatterns = _options.DangerousPatterns;

            // 检查查询参数
            foreach (var param in context.Request.Query)
            {
                if (ContainsDangerousPattern(param.Value, dangerousPatterns))
                    return true;
            }

            // 检查表单数据
            if (context.Request.HasFormContentType)
            {
                foreach (var param in context.Request.Form)
                {
                    if (ContainsDangerousPattern(param.Value, dangerousPatterns))
                        return true;
                }
            }

            return false;
        }

        private bool ContainsDangerousPattern(string input, IEnumerable<string> patterns)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return patterns.Any(pattern =>
                input.Contains(pattern, StringComparison.OrdinalIgnoreCase));
        }

        private string GetClientIp(HttpContext context)
        {
            var xForwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(xForwardedFor))
            {
                return xForwardedFor.Split(',')[0].Trim();
            }

            var xRealIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(xRealIp))
            {
                return xRealIp;
            }

            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }

    public class RequestValidationOptions
    {
        public bool EnableRateLimit { get; set; } = true;
        public int RateLimitPerMinute { get; set; } = 100;
        public long MaxRequestSize { get; set; } = 10 * 1024 * 1024; // 10MB
        public List<string> DangerousPatterns { get; set; } = new()
        {
            "<script", "javascript:", "eval(", "alert(", "confirm(",
            "prompt(", "document.cookie", "document.write", ".innerHTML",
            "union select", "drop table", "delete from", "update set",
            "../", "..\\", "%2e%2e", "%252e%252e"
        };
    }
}