using FlyFramework.FlyFrameworkModules.Extensions;
using FlyFramework.FlyFrameworkModules.Interface;
using FlyFramework.FlyFrameworkModules.Modules;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace FlyFramework.FlyFrameworkModules
{
    public class FlyFrameworkBaseModuleApplication : IFlyFrameworkModuleApplication
    {
        public Type StartModuleType { get; private set; }
        public IServiceCollection Services { get; private set; }
        public IServiceProvider ServiceProvider { get; private set; }
        public IReadOnlyList<IFlyFrameworkBaseModuleDescritor> Modules { get; private set; }

        private bool isConfigService;

        /// <summary>
        /// 初始化 BaseModuleApplication 实例。
        /// </summary>
        /// <param name="startModuleType">启动模块类型</param>
        /// <param name="services">服务集合</param>
        public FlyFrameworkBaseModuleApplication(Type startModuleType, IServiceCollection services)
        {
            var moduleLoader = new FlyFrameworkModuleLoad();
            FlyFrameworkBaseModule.CheckModuleType(startModuleType);
            services.CheckNull();
            StartModuleType = startModuleType;
            Services = services;
            isConfigService = false;

            services.AddSingleton<IFlyFrameworkModuleLoad>(moduleLoader);

            // 初始化 ServiceProvider
            ServiceProvider = services.BuildServiceProvider();

            services.AddObjectAccessor<IServiceProvider>();
            Services.AddObjectAccessor<InitApplicationContext>();
            Services.AddSingleton<IFlyFrameworkModuleContainer>(this);
            Services.AddSingleton<IFlyFrameworkModuleApplication>(this);

            Modules = LoadModules(services);

            Initialize();
        }

        /// <summary>
        /// 配置服务。
        /// </summary>
        public virtual void Initialize()
        {
            if (isConfigService) return;

            ServiceConfigerContext context = new ServiceConfigerContext(Services);

            foreach (var module in Modules)
            {
                if (module.Instance is FlyFrameworkBaseModule baseModule)
                {
                    baseModule.ServiceConfigerContext = context;
                }
            }

            // 预初始化应用程序
            try
            {
                foreach (var module in Modules.Where(m => m.Instance is IPreInitialize))
                {
                    try
                    {
                        module.Instance.PreInitialize(context);
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException(
                            $"An error occurred during the {nameof(IPreInitialize.PreInitialize)} phase of the module {module.ModuleType.AssemblyQualifiedName}.", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("An error occurred during the pre-configuration phase.", ex);
            }

            // 配置服务
            try
            {
                foreach (var module in Modules)
                {
                    if (module.Instance is FlyFrameworkBaseModule baseModule)
                    {
                        // 继承生命周期接口的类进行自动注册
                        Services.AddAssembly(module.ModuleType.Assembly);
                    }
                    try
                    {
                        module.Instance.Initialize(context);
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException(
                            $"An error occurred during the {nameof(IFlyFrameworkBaseModule.Initialize)} phase of the module {module.ModuleType.AssemblyQualifiedName}.", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("An error occurred during the configuration phase.", ex);
            }

            // 初始化后应用程序
            try
            {
                foreach (var module in Modules)
                {
                    try
                    {
                        module.Instance.PostInitialize(context);
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException(
                            $"An error occurred during the {nameof(IFlyFrameworkBaseModule.PostInitialize)} phase of the module {module.ModuleType.AssemblyQualifiedName}.", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("An error occurred during the post-configuration phase.", ex);
            }

            isConfigService = true;
        }

        /// <summary>
        /// 设置服务提供者。
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        protected virtual void SetServiceProvider(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            ServiceProvider.GetRequiredService<ObjectAccessor<IServiceProvider>>().Value = serviceProvider;
        }

        /// <summary>
        /// 初始化应用程序。
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        public virtual void InitApplication(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            scope.ServiceProvider
                .GetRequiredService<IFlyFrameworkModuleManager>()
                .InitializeModules();
        }

        /// <summary>
        /// 异步初始化应用程序。
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        public virtual async Task InitApplicationAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            await scope.ServiceProvider
               .GetRequiredService<IFlyFrameworkModuleManager>()
               .InitializeModulesAsync();
        }

        /// <summary>
        /// 加载模块。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>模块描述符列表</returns>
        protected virtual IReadOnlyList<IFlyFrameworkBaseModuleDescritor> LoadModules(IServiceCollection services)
        {
            return services
                .GetSingletonInstance<IFlyFrameworkModuleLoad>()
                .GetModuleDescritors(services, StartModuleType);
        }
    }
}