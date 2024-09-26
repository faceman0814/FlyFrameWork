using Autofac;
using Autofac.Core;

using AutoMapper;

using FlyFramework.Application;
using FlyFramework.Application.UserService;
using FlyFramework.Common.Attributes;
using FlyFramework.Domain.ApplicationServices;
using FlyFramework.Repositories.UserSessions;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

using System.ComponentModel;
using System.Reflection;

using Module = Autofac.Module;

namespace FlyFramework.WebHost.Autofac
{
    /// <summary>
    /// 容器注册类
    /// </summary>
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // 注册 UserSession 实现到 IUserSession 接口
            builder.RegisterType<UserSession>()
                   .As<IUserSession>()
                   .InstancePerLifetimeScope();

            builder.RegisterType<Mapper>()
                  .As<IMapper>()
                  .SingleInstance();


            // 注册所有应用服务，并开启属性注入
            builder.RegisterAssemblyTypes(typeof(FlyFrameworkApplicationModule).Assembly)
                   .Where(t => t.Name.EndsWith("AppService"))
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


