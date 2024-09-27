using Autofac;

using FlyFramework.Application;
using FlyFramework.Common.FlyFrameworkModules.Modules;
using FlyFramework.Core;
using FlyFramework.WebHost.Controllers;

namespace FlyFramework.WebHost
{
    [DependOn(typeof(FlyFrameworkApplicationModule))]
    public class FlyFrameworkWebHostModule : FlyFrameworkBaseModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            // 注册 HomeController
            builder.RegisterType<HomeController>().InstancePerLifetimeScope();
            builder.RegisterType<LoginController>().InstancePerLifetimeScope();
        }
    }
}
