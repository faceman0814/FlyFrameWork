using FlyFramework.Common.Dependencys;

namespace FlyFramework.Common.FlyFrameworkModules.Interface
{
    public interface IFlyFrameworkModuleLifecycleContributor : ITransientDependency
    {
        void Initialize(InitApplicationContext context, IFlyFrameworkBaseModule module);

        Task InitializeAsync(InitApplicationContext context, IFlyFrameworkBaseModule module);
    }
}