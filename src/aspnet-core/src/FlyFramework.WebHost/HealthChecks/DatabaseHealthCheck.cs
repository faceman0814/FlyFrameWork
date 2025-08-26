using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FlyFramework.HealthChecks
{
    /// <summary>
    /// 数据库健康检查
    /// </summary>
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<DatabaseHealthCheck> _logger;

        public DatabaseHealthCheck(IServiceScopeFactory serviceScopeFactory, ILogger<DatabaseHealthCheck> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

                // 简单的连接性检查
                var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);

                if (canConnect)
                {
                    // 可以添加更详细的检查，如执行简单查询
                    var connectionInfo = dbContext.Database.GetConnectionString();
                    _logger.LogDebug("数据库连接正常: {ConnectionString}", MaskConnectionString(connectionInfo));

                    return HealthCheckResult.Healthy("数据库连接正常");
                }
                else
                {
                    _logger.LogWarning("数据库连接失败");
                    return HealthCheckResult.Unhealthy("无法连接到数据库");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "数据库健康检查时发生异常");
                return HealthCheckResult.Unhealthy("数据库健康检查异常", ex);
            }
        }

        private string MaskConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                return "N/A";

            // 简单的密码掩码
            return System.Text.RegularExpressions.Regex.Replace(
                connectionString,
                @"(password|pwd)\s*=\s*[^;]*",
                "$1=***",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }
    }
}