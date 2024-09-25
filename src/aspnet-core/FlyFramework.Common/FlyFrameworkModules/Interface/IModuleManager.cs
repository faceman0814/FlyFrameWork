namespace FlyFramework.Common.FlyFrameworkModules.Interface
{
    public interface IModuleManager
    {
        void InitializeModules();

        Task InitializeModulesAsync();
    }
}