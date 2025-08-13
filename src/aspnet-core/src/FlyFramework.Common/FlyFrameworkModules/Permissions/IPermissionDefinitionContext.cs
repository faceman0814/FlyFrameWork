using System.Collections.Generic;

namespace FlyFramework.FlyFrameworkModules.Permissions
{
    public interface IPermissionDefinitionContext
    {
        Permission CreatePermission(string name, string displayName = null, Dictionary<string, object> properties = null);
        Permission GetPermissionOrNull(string name);
        void  RemovePermission(string name);
    }
}
