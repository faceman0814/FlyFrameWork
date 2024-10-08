using Autofac;
using Autofac.Core;

using AutoMapper;

using FlyFramework.Attributes;
using FlyFramework.DynamicWebAPI;
using FlyFramework.FlyFrameworkModules;
using FlyFramework.FlyFrameworkModules.Modules;
using FlyFramework.Uow;
using FlyFramework.UserService.Dtos;
using FlyFramework.UserService.Mappers;
using FlyFramework.UserSessions;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using ServiceStack;

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

            //AddMapper(builder);

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
