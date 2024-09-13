using FlyFramework.Common.Dependencys;
using FlyFramework.Common.Domain;

using System.Reflection;

namespace FlyFramework.WebHost.DI
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDependencyServices(this IServiceCollection services)
        {
            // 获取当前应用程序域中已加载的以 "FlyFramework" 开头的程序集
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(t => t.FullName.StartsWith("FlyFramework")).ToArray();

            // 遍历符合条件的程序集
            foreach (var assembly in assemblies)
            {
                // 扫描程序集中所有非抽象类类型
                var types = assembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract);

                foreach (var type in types)
                {
                    var interfaces = type.GetInterfaces();
                    var dependencyInterfaces = interfaces.Intersect(new[] { typeof(ITransientDependency), typeof(IScopedDependency), typeof(ISingletonDependency) });

                    if (!dependencyInterfaces.Any()) continue;

                    // 遍历符合条件的依赖接口并注册到服务容器中
                    foreach (var serviceType in dependencyInterfaces)
                    {
                        if (typeof(ITransientDependency).IsAssignableFrom(serviceType))
                        {
                            services.AddTransient(serviceType, type);
                        }
                        else if (typeof(IScopedDependency).IsAssignableFrom(serviceType))
                        {
                            services.AddScoped(serviceType, type);
                        }
                        else if (typeof(ISingletonDependency).IsAssignableFrom(serviceType))
                        {
                            services.AddSingleton(serviceType, type);
                        }
                    }
                }
            }
            return services;
        }

        public static IServiceCollection AddManagerRegisterServices(this IServiceCollection services)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
               .Where(t => t.FullName.StartsWith("FlyFramework"))
               .ToArray();

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes()
                    .Where(type => type.IsClass && !type.IsAbstract);

                foreach (var type in types)
                {
                    var interfaces = type.GetInterfaces().Where(t => t.Name.EndsWith("Manager")).Distinct();

                    // 自动注册与接口名称匹配的服务实现
                    foreach (var interfaceType in interfaces)
                    {
                        Console.WriteLine("扫描: {0}", interfaceType.Name);
                        if (!interfaceType.IsPublic || interfaceType == typeof(IDisposable))
                            continue;

                        if (interfaceType.Name.Equals($"I{type.Name}", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("AutoRegister: {0}", interfaceType.Name);
                            RegisterService(services, interfaceType, type);
                        }
                    }
                }
            }

            return services;
        }

        private static void RegisterService(IServiceCollection services, Type interfaceType, Type implementationType)
        {
            // 默认注册为 Scoped，可根据需要调整
            services.AddScoped(interfaceType, implementationType);
        }

        private static void AutoRegisterByLifecycle(IServiceCollection services, Type type, IEnumerable<Type> interfaces)
        {
            var lifecycleInterfaces = new[]
            {
            typeof(ITransientDependency),
            typeof(IScopedDependency),
            typeof(ISingletonDependency)
        };

            var matchedInterfaces = interfaces.Intersect(lifecycleInterfaces);
            foreach (var serviceType in matchedInterfaces)
            {
                if (typeof(ITransientDependency).IsAssignableFrom(serviceType))
                {
                    services.AddTransient(type);
                }
                else if (typeof(IScopedDependency).IsAssignableFrom(serviceType))
                {
                    services.AddScoped(type);
                }
                else if (typeof(ISingletonDependency).IsAssignableFrom(serviceType))
                {
                    services.AddSingleton(type);
                }
            }
        }
    }

}
