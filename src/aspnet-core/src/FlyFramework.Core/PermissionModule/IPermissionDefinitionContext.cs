using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.PermissionModule
{
    public interface IPermissionDefinitionContext
    {
        Permission CreatePermission(string name, string displayName = null, Dictionary<string, object> properties = null);
        Permission GetPermissionOrNull(string name);
        void  RemovePermission(string name);
    }
}
