using FlyFramework.Common.Dependencys;

namespace FlyFramework.Common.FlyFrameworkModules.Interface
{
    public interface IModuleLifecycleContributor : ITransientDependency
    {
        void Initialize(InitApplicationContext context, IBaseModule module);

        Task InitializeAsync(InitApplicationContext context, IBaseModule module);
    }
}