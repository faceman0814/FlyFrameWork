using FlyFramework.FlyFrameworkModules.Modules;
using FlyFramework.FlyFrameworkModules.Permissions;
using FlyFramework.LazyModule.LazyDefinition;
using FlyFramework.PermissionModule;
using FlyFramework.Repositories;

using Microsoft.Extensions.DependencyInjection;

using ServiceStack;

namespace FlyFramework
{
    [DependOn(typeof(FlyFrameworkDomainModule))]
    public class FlyFrameworkCoreModule : FlyFrameworkBaseModule
    {
        public override void Initialize()
        {
            Configuration.Services.AddTransient(typeof(IFlyFrameworkLazy), typeof(FlyFrameworkLazy));
            Configuration.Services.AddTransient(typeof(IPermissionDefinitionContext), typeof(PermissionDefinitionContext));
            Configuration.Services.AddTransient(typeof(IRepository<PermissionSetting, string>), typeof(Repository<PermissionSetting, string>));
        }

    }
}
