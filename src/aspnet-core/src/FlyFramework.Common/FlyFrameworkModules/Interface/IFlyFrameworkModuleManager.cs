using System.Threading.Tasks;
namespace FlyFramework.FlyFrameworkModules.Interface
{
    public interface IFlyFrameworkModuleManager
    {
        void InitializeModules();

        Task InitializeModulesAsync();
    }
}