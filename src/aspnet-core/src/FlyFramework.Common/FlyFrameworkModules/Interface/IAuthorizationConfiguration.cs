using FlyFramework.FlyFrameworkModules.Collections;
using FlyFramework.FlyFrameworkModules.Permissions;

namespace FlyFramework.FlyFrameworkModules.Interface
{
    public interface IAuthorizationConfiguration
    {
        ITypeList<AuthorizationProvider> Providers { get; }

        bool IsEnabled { get; set; }
    }
}
