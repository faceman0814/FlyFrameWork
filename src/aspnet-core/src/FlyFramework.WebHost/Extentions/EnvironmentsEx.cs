using Microsoft.Extensions.Hosting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Extentions
{
    public static class EnvironmentExs
    {
        /// <summary>
        /// 指定开发环境。
        /// </summary>
        /// <remarks>开发环境可以实现不应在生产中暴露的功能。由于性能成本，范围验证和依赖验证仅发生在开发中。</remarks>
        public static readonly string Development = "Development";
        /// <summary>
        /// 指定预发布环境。
        /// </summary>
        /// <remarks>预发布环境可用于在更改生产环境之前验证应用程序更改。</remarks>
        public static readonly string Staging = "Staging";
        /// <summary>
        /// 指定生产环境
        /// </summary>
        /// <remarks>生产环境应配置为最大化安全性，性能和应用程序可用性。</remarks>
        public static readonly string Production = "Production";
        /// <summary>
        /// 指定测试环境
        /// </summary>
        public static readonly string Testing = "Testing";
    }
}
