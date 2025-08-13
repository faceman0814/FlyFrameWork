using FlyFramework.FlyFrameworkModules;

namespace FlyFramework.FlyFrameworkModules.Interface
{
    public interface IFlyFrameworkBaseModule : IPreInitialize
    {
        void Initialize();

        void InitApplication();

        void PostInitialize();
    }
}