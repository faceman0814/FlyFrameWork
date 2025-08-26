using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Middlewares
{
    /// <summary>
    /// 性能监控中间件
    /// </summary>
    public class PerformanceMonitoringMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PerformanceMonitoringMiddleware> _logger;

        public PerformanceMonitoringMiddleware(RequestDelegate next, ILogger<PerformanceMonitoringMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestBody = string.Empty;
            var responseBody = string.Empty;

            try
            {
                // 记录请求体（仅限于POST/PUT等有Body的请求）
                if (ShouldLogRequestBody(context))
                {
                    requestBody = await ReadRequestBodyAsync(context);
                }

                // 包装响应流以捕获响应内容
                var originalResponseStream = context.Response.Body;
                using var responseMemoryStream = new MemoryStream();
                context.Response.Body = responseMemoryStream;

                await _next(context);

                stopwatch.Stop();

                // 读取响应内容
                responseBody = await ReadResponseBodyAsync(responseMemoryStream);

                // 恢复原始响应流
                responseMemoryStream.Seek(0, SeekOrigin.Begin);
                await responseMemoryStream.CopyToAsync(originalResponseStream);
                context.Response.Body = originalResponseStream;

                // 记录性能信息
                await LogPerformanceAsync(context, stopwatch.ElapsedMilliseconds, requestBody, responseBody);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "请求处理过程中发生异常，耗时: {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        private bool ShouldLogRequestBody(HttpContext context)
        {
            var method = context.Request.Method.ToUpperInvariant();
            return (method == "POST" || method == "PUT" || method == "PATCH") &&
                   context.Request.ContentType?.Contains("application/json") == true;
        }

        private async Task<string> ReadRequestBodyAsync(HttpContext context)
        {
            try
            {
                context.Request.EnableBuffering();
                context.Request.Body.Position = 0;

                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

                // 限制日志长度，避免过长的请求体
                return body.Length > 1000 ? body.Substring(0, 1000) + "..." : body;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "读取请求体时发生异常");
                return "无法读取请求体";
            }
        }

        private async Task<string> ReadResponseBodyAsync(MemoryStream stream)
        {
            try
            {
                stream.Position = 0;
                using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
                var content = await reader.ReadToEndAsync();

                // 限制日志长度
                return content.Length > 1000 ? content.Substring(0, 1000) + "..." : content;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "读取响应体时发生异常");
                return "无法读取响应体";
            }
        }

        private Task LogPerformanceAsync(HttpContext context, long elapsedMilliseconds, string requestBody, string responseBody)
        {
            var logLevel = GetLogLevel(elapsedMilliseconds, context.Response.StatusCode);

            var requestInfo = new
            {
                Method = context.Request.Method,
                Path = context.Request.Path,
                QueryString = context.Request.QueryString.ToString(),
                UserAgent = context.Request.Headers.UserAgent.ToString(),
                ClientIP = GetClientIp(context),
                RequestBody = requestBody,
                ResponseBody = responseBody,
                StatusCode = context.Response.StatusCode,
                ElapsedMilliseconds = elapsedMilliseconds,
                Timestamp = DateTime.UtcNow
            };

            _logger.Log(logLevel, "请求处理完成: {@RequestInfo}", requestInfo);

            // 如果响应时间过长，记录警告
            if (elapsedMilliseconds > 5000)
            {
                _logger.LogWarning("慢请求警告: {Method} {Path} 耗时 {ElapsedMilliseconds}ms",
                    context.Request.Method, context.Request.Path, elapsedMilliseconds);
            }

            return Task.CompletedTask;
        }

        private LogLevel GetLogLevel(long elapsedMilliseconds, int statusCode)
        {
            // 根据响应时间和状态码确定日志级别
            if (statusCode >= 500) return LogLevel.Error;
            if (statusCode >= 400) return LogLevel.Warning;
            if (elapsedMilliseconds > 2000) return LogLevel.Warning;
            if (elapsedMilliseconds > 1000) return LogLevel.Information;
            return LogLevel.Debug;
        }

        private string GetClientIp(HttpContext context)
        {
            var xForwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(xForwardedFor))
            {
                return xForwardedFor.Split(',')[0].Trim();
            }

            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }
}