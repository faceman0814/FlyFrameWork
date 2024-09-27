using Autofac;
using Autofac.Core;

using AutoMapper;

using FlyFramework.Application.UserService.Mappers;
using FlyFramework.Common.Attributes;
using FlyFramework.Common.FlyFrameworkModules.Modules;
using FlyFramework.Core;
using FlyFramework.Repositories.Uow;
using FlyFramework.Repositories.UserSessions;

using System.Linq;
using System.Reflection;

namespace FlyFramework.Application
{
    [DependOn(typeof(FlyFrameworkCoreModule))]
    public class FlyFrameworkApplicationModule : FlyFrameworkBaseModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            //ApplicationService的属性注入
            builder.RegisterType<UserSession>()
                   .As<IUserSession>()
                   .InstancePerLifetimeScope();

            builder.RegisterType<UnitOfWorkManager>()
                  .As<IUnitOfWorkManager>()
                  .InstancePerLifetimeScope();

            AddMapper(builder);

            // 注册所有应用服务，并开启属性注入
            builder.RegisterAssemblyTypes(typeof(FlyFrameworkApplicationModule).Assembly)
                   .Where(t => t.Name.EndsWith("AppService"))
                   //.EnableClassInterceptors() // 如果使用拦截器
                   .PropertiesAutowired(new IocSelectPropertySelector()); // 启用属性注入
        }
        public static void AddMapper(ContainerBuilder builder)
        {
            // AutoMapper 配置
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new UserMapper());
            });
            IMapper mapper = mapperConfig.CreateMapper();
            builder.RegisterInstance(mapper).As<IMapper>().SingleInstance();
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
