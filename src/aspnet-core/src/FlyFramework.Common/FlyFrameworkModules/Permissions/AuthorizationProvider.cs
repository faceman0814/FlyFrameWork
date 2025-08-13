using FlyFramework.Dependencys;
namespace FlyFramework.FlyFrameworkModules.Permissions
{
    public abstract class AuthorizationProvider : ITransientDependency
    {
        public abstract void SetPermissions(IPermissionDefinitionContext context);
    }
}
