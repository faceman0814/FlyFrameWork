using FlyFramework.FlyFrameworkModules;

namespace FlyFramework.FlyFrameworkModules.Interface
{
    public interface IFlyFrameworkBaseModule : IPreInitialize
    {
        void Initialize(ServiceConfigerContext context);

        void InitApplication(InitApplicationContext context);

        void PostInitialize(InitApplicationContext context);
    }
}