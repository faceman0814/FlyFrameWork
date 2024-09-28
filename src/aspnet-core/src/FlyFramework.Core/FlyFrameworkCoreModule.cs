
using FlyFramework.Common.Dependencys;
using FlyFramework.Common.FlyFrameworkModules;
using FlyFramework.Common.FlyFrameworkModules.Modules;
using FlyFramework.Core.LazyModule.LazyDefinition;

using Microsoft.Extensions.DependencyInjection;

using ServiceStack;

using System;
using System.Linq;

namespace FlyFramework.Core
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
