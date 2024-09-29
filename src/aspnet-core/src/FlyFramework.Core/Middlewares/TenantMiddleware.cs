using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Middlewares
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var host = context.Request.Host.Value; // 获取主机名
            var tenantId = GetTenantIdFromHost(host); // 根据主机名提取租户ID
            context.Items["TenantId"] = tenantId; // 将租户ID存储在HttpContext中

            await _next(context);
        }

        private int GetTenantIdFromHost(string host)
        {
            // TODO: 从主机名解析租户ID
            return 1; // 示例返回值，实际应根据情况实现
        }
    }
}
