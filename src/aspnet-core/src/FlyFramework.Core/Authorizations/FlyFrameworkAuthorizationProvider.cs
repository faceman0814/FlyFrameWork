using FlyFramework.PermissionModule;

using ServiceStack;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Authorizations
{
    public class FlyFrameworkAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context.CreatePermission(PermissionNames.Pages_Users, "Users");
            context.CreatePermission(PermissionNames.Pages_Roles, "Roles");
        }
    }

}
