using Autofac.Core;

using FlyFramework.FlyFrameworkModules;
using FlyFramework.FlyFrameworkModules.Modules;
using FlyFramework.LazyModule.LazyDefinition;
using FlyFramework.PermissionModule;
using FlyFramework.PermissionModule.DomainService;
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
            Configuration.Services.AddTransient(typeof(IRepository<Permission, string>), typeof(Repository<Permission, string>));
        }

    }
}
