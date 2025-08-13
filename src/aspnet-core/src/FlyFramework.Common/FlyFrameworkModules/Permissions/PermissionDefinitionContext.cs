using FlyFramework.ErrorExceptions;
using FlyFramework.Extentions;

using System;
using System.Collections.Generic;

namespace FlyFramework.FlyFrameworkModules.Permissions;
public class PermissionDefinitionContext : IPermissionDefinitionContext
{
    public PermissionDefinitionContext(IServiceProvider serviceProvider)
    {
    }

    public Permission CreatePermission(string name, string displayName = null, Dictionary<string, object> properties = null)
    {
        Check.NotNull(name, nameof(name));
        var permission = new Permission(name, displayName);
        return permission;
    }

    public Permission GetPermissionOrNull(string name)
    {
        //Check.NotNull(name, nameof(name));
        //return _permissionManager.GetAll().FirstOrDefault(t => t.Name == name);
        return null;
    }

    public void RemovePermission(string name)
    {
        Check.NotNull(name, nameof(name));
        var permission = GetPermissionOrNull(name);
        if (permission == null)
        {
            throw new EntityNotFoundException(typeof(Permission), name);
        }
        //_permissionManager.Delete(permission.Id);
    }
}
