using Autofac;

using FlyFramework.Controllers;
using FlyFramework.FlyFrameworkModules.Modules;

namespace FlyFramework
{
    [DependOn(typeof(FlyFrameworkWebCoreModule))]
    public class FlyFrameworkWebHostModule : FlyFrameworkBaseModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HomeController>().InstancePerLifetimeScope();
            builder.RegisterType<AccountClientController>().InstancePerLifetimeScope();
        }
    }
}
