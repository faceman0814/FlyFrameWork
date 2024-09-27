using FlyFramework.Common.FlyFrameworkModules.Interface;

using System.Threading.Tasks;
namespace FlyFramework.Common.FlyFrameworkModules.Modules
{
    public abstract class FlyFrameworkModuleLifecycleContributor : IFlyFrameworkModuleLifecycleContributor
    {
        public virtual void Initialize(InitApplicationContext context, IFlyFrameworkBaseModule module)
        {
        }

        public virtual Task InitializeAsync(InitApplicationContext context, IFlyFrameworkBaseModule module)
        {
            return Task.CompletedTask;
        }
    }
}