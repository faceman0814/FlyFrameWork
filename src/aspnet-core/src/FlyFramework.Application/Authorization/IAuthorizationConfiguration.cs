using FlyFramework.Authorizations;
using FlyFramework.FlyFrameworkModules.Collections;

namespace FlyFramework.Authorization
{
    public interface IAuthorizationConfiguration
    {
        ITypeList<AuthorizationProvider> Providers { get; }

        bool IsEnabled { get; set; }
    }

    public class AuthorizationConfiguration : IAuthorizationConfiguration
    {
        public ITypeList<AuthorizationProvider> Providers { get; } = new TypeList<AuthorizationProvider>();
        public bool IsEnabled { get; set; } = true;
    }
}
