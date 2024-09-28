using System.Collections.Generic;

namespace FlyFramework.FlyFrameworkModules.Interface
{
    public interface IFlyFrameworkModuleContainer
    {
        IReadOnlyList<IFlyFrameworkBaseModuleDescritor> Modules { get; }
    }
}