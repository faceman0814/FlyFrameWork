using FlyFramework.Common.FlyFrameworkModules.Interface;
using FlyFramework.Common.FlyFrameworkModules.Modules;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;

namespace FlyFramework.Common.FlyFrameworkModules.Extensions
{
    public static class InitializeContextExtensions
    {
        /// <summary>
        /// 获取Configuration
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IConfiguration GetConfiguration(this ServiceConfigerContext context)
        {
            if (context == null && context.Services is null) throw new ArgumentException("ServiceConfigerContext is null");
            return context.Provider.GetRequiredService<IConfiguration>();
        }

        /// <summary>
        /// 获取静态文件
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IHostEnvironment Environment(this ServiceConfigerContext context)
        {
            if (context is null || context.Services is null) throw new ArgumentException("context is null");
            return context.Provider.GetRequiredService<IHostEnvironment>();
        }

        /// <summary>
        /// 服务注册
        /// </summary>
        /// <typeparam name="TMoudel"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddApplication<TMoudel>(this IServiceCollection services) where TMoudel : FlyFrameworkBaseModule
        {
            services.ChcekNull();
            services.AddSingleton<IFlyFrameworkModuleManager, FlyFrameworkModuleManager>();
            services.AddObjectAccessor<IApplicationBuilder>();
            new FlyFrameworkBaseModuleApplicationServiceProvider(typeof(TMoudel), services);
            return services;
        }
    }
}