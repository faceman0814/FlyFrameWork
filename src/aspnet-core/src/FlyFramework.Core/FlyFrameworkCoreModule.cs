using Autofac;

using FlyFramework.FlyFrameworkModules;
using FlyFramework.FlyFrameworkModules.Modules;
using FlyFramework.LazyModule.LazyDefinition;

using Microsoft.Extensions.DependencyInjection;

using ServiceStack;

namespace FlyFramework
{
    [DependOn(typeof(FlyFrameworkDomainModule))]
    public class FlyFrameworkCoreModule : FlyFrameworkBaseModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FlyFrameworkLazy>().As<IFlyFrameworkLazy>().InstancePerDependency();
            // 注册所有应用服务，并开启属性注入
            builder.RegisterAssemblyTypes(typeof(FlyFrameworkCoreModule).Assembly)
                   //.Where(t => t.Name.EndsWith("AppService"))
                   //.EnableClassInterceptors() // 如果使用拦截器
                   .PropertiesAutowired(); // 启用属性注入
        }
    }
}
