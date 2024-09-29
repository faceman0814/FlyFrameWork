using FlyFramework.Dependencys;
using FlyFramework.FlyFrameworkModules;

using System.Threading.Tasks;
namespace FlyFramework.FlyFrameworkModules.Interface
{
    public interface IFlyFrameworkModuleLifecycleContributor : ITransientDependency
    {
        void Initialize(InitApplicationContext context, IFlyFrameworkBaseModule module);

        Task InitializeAsync(InitApplicationContext context, IFlyFrameworkBaseModule module);
    }
}