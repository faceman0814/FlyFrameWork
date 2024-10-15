using FlyFramework.FlyFrameworkModules;

namespace FlyFramework.FlyFrameworkModules.Interface
{
    public interface IPreInitialize
    {
        void PreInitialize(ServiceConfigerContext context);
    }
}