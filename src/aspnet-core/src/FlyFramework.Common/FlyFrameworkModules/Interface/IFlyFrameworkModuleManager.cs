using System.Threading.Tasks;
namespace FlyFramework.Common.FlyFrameworkModules.Interface
{
    public interface IFlyFrameworkModuleManager
    {
        void InitializeModules();

        Task InitializeModulesAsync();
    }
}