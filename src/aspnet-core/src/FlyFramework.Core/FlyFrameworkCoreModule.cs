
using FlyFramework.Common.FlyFrameworkModules;
using FlyFramework.Common.FlyFrameworkModules.Modules;
using FlyFramework.Core.LazyModule.LazyDefinition;

using Microsoft.Extensions.DependencyInjection;

namespace FlyFramework.Core
{
    public class FlyFrameworkCoreModule : FlyFrameworkBaseModule
    {
        public override void Initialize(ServiceConfigerContext context)
        {
            context.Services.AddTransient(typeof(IFlyFrameworkLazy<>), typeof(FlyFrameworkLazy<>));
        }
    }
}
