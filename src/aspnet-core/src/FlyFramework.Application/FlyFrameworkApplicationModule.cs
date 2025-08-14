
using Autofac;
using Autofac.Core;

using AutoMapper;

using Castle.Core.Logging;

using FlyFramework.Attributes;
using FlyFramework.Authorization;
using FlyFramework.FlyFrameworkModules.Modules;
using FlyFramework.OrgUnitModule.Authorization;
using FlyFramework.OrgUnitModule.OrgUnitNodes.Mappers;
using FlyFramework.RoleModule.Mappers;
using FlyFramework.Uow;
using FlyFramework.UserModule.Mappers;
using FlyFramework.UserSessions;

using Microsoft.Extensions.DependencyInjection;

using ServiceStack;

using System.Linq;
using System.Reflection;

using static System.Net.Mime.MediaTypeNames;

namespace FlyFramework
{


    [DependOn(typeof(FlyFrameworkCoreModule))]
    public class FlyFrameworkApplicationModule : FlyFrameworkBaseModule
    {
        public override void PreInitialize()
        {
            ConfigurePermissionProvider();
            ConfigureAutoMapper();
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

            builder.Register(c => new ConsoleLogger())
                .As<ILogger>()
                .InstancePerLifetimeScope();

            // 注册所有应用服务，并开启属性注入
            builder.RegisterAssemblyTypes(typeof(FlyFrameworkApplicationModule).Assembly)
                   //.Where(t => t.Name.EndsWith("AppService"))
                   //.EnableClassInterceptors() // 如果使用拦截器
                   .PropertiesAutowired(new IocSelectPropertySelector()); // 启用属性注入
        }

        private void ConfigurePermissionProvider()
        {
            var Authorization = new AuthorizationConfiguration();
            Configuration.Services.AddSingleton<IAuthorizationConfiguration, AuthorizationConfiguration>();

            Authorization.Providers.Add<OrgUnitAuthorizationProvider>();
        }

        private void ConfigureAutoMapper()
        {
            // 配置 AutoMapper
            Configuration.Services.AddAutoMapper((serviceProvider, configuration) =>
            {
                UserMapper.CreateMappings(configuration);
                OrgUnitNodeMapper.CreateMappings(configuration);
                RoleMapper.CreateMappings(configuration);
            }, typeof(FlyFrameworkApplicationModule));
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
