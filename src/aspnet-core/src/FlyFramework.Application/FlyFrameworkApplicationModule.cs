
using Autofac;
using Autofac.Core;

using AutoMapper;

using Castle.Core.Logging;

using FlyFramework.Attributes;
using FlyFramework.Authorizations;
using FlyFramework.FlyFrameworkModules;
using FlyFramework.FlyFrameworkModules.Modules;
using FlyFramework.Uow;
using FlyFramework.UserModule.Mappers;
using FlyFramework.UserSessions;

using Microsoft.Extensions.DependencyInjection;

using System.Linq;
using System.Reflection;

namespace FlyFramework
{
    [DependOn(typeof(FlyFrameworkCoreModule))]
    public class FlyFrameworkApplicationModule : FlyFrameworkBaseModule
    {
        public override void PreInitialize(ServiceConfigerContext context)
        {
            // 配置 AutoMapper
            context.Services.AddAutoMapper((serviceProvider, configuration) =>
            {
                UserMapper.CreateMappings(configuration);

            }, typeof(FlyFrameworkApplicationModule));
        }

        public override void Initialize(ServiceConfigerContext context)
        {

        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UserSession>()
                   .As<IUserSession>()
                   .InstancePerLifetimeScope();

            builder.RegisterType<UnitOfWorkManager>()
                  .As<IUnitOfWorkManager>()
                  .InstancePerLifetimeScope();

            builder.RegisterType<Mapper>()
                 .As<IMapper>()
                 .SingleInstance();

            builder.RegisterType<LogInManager>()
                 .As<ILogInManager>()
                 .SingleInstance();

            builder.Register(c => new ConsoleLogger())
                .As<ILogger>()
                .InstancePerLifetimeScope();

            // 注册所有应用服务，并开启属性注入
            builder.RegisterAssemblyTypes(typeof(FlyFrameworkApplicationModule).Assembly)
                   //.Where(t => t.Name.EndsWith("AppService"))
                   //.EnableClassInterceptors() // 如果使用拦截器
                   .PropertiesAutowired(new IocSelectPropertySelector()); // 启用属性注入
        }

    }

    /// <summary>
    /// 属性注入选择器
    /// </summary>
    public class IocSelectPropertySelector : IPropertySelector
    {
        public bool InjectProperty(PropertyInfo propertyInfo, object instance)
        {
            // 带有 AutowiredAttribute 特性的属性会进行属性注入
            return propertyInfo.CustomAttributes.Any(it => it.AttributeType == typeof(IocSelectAttribute));
        }
    }
}
