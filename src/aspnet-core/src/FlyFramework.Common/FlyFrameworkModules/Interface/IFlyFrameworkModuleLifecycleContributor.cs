using System.Threading.Tasks;
namespace FlyFramework.FlyFrameworkModules.Interface
{
    public interface IFlyFrameworkModuleLifecycleContributor
    {
        void Initialize(InitApplicationContext context, IFlyFrameworkBaseModule module);

        Task InitializeAsync(InitApplicationContext context, IFlyFrameworkBaseModule module);
    }
}