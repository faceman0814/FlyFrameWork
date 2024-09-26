using FlyFramework.Common.FlyFrameworkModules.Collections;
using FlyFramework.Common.FlyFrameworkModules.Interface;

namespace FlyFramework.Common.FlyFrameworkModules.Modules
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