using FlyFramework.Common.FlyFrameworkModules.Interface;

namespace FlyFramework.Common.FlyFrameworkModules.Modules
{
    public abstract class ModuleLifecycleContributor : IModuleLifecycleContributor
    {
        public virtual void Initialize(InitApplicationContext context, IBaseModule module)
        {
        }

        public virtual Task InitializeAsync(InitApplicationContext context, IBaseModule module)
        {
            return Task.CompletedTask;
        }
    }
}