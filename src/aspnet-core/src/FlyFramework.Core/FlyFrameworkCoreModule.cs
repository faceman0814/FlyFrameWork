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
        public override void Initialize(ServiceConfigerContext context)
        {
            //IocManager.AddDependencyServices(context.Services, typeof(FlyFrameworkCoreModule).Assembly, InterfacePostfixes);

            context.Services.AddTransient(typeof(IFlyFrameworkLazy<>), typeof(FlyFrameworkLazy<>));
        }
    }
}
