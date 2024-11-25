using FlyFramework.Dependencys;
using FlyFramework.PermissionModule;
namespace FlyFramework.Authorizations
{
    public abstract class AuthorizationProvider : ITransientDependency
    {
        public abstract void SetPermissions(IPermissionDefinitionContext context);
    }
}
