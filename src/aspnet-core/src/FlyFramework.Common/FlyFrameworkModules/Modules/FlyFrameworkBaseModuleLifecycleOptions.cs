using FlyFramework.FlyFrameworkModules.Collections;
using FlyFramework.FlyFrameworkModules.Interface;

namespace FlyFramework.FlyFrameworkModules.Modules
{
    public class FlyFrameworkBaseModuleLifecycleOptions
    {
        public ITypeList<IFlyFrameworkModuleLifecycleContributor> Contributors { get; }

        public FlyFrameworkBaseModuleLifecycleOptions()
        {
            Contributors = new TypeList<IFlyFrameworkModuleLifecycleContributor>();
        }
    }
}