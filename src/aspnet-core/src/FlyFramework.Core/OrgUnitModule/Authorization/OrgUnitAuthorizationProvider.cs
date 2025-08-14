using FlyFramework.Authorizations;
using FlyFramework.PermissionModule;
using FlyFramework.UserModule.Authorization;

using ServiceStack.Auth;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.OrgUnitModule.Authorization
{
    public class OrgUnitAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var pages = context.GetPermissionOrNull(AppPermissions.Pages) ??
                         context.CreatePermission(AppPermissions.Pages, "页面");

            var userManagement = pages.Children.FirstOrDefault(p => p.Name == UserPermissions.UserManager) ??
                          pages.CreateChildPermission(OrgUnitPermissions.OrgUnit_Node, OrgUnitPermissions.OrgUnit_Node_Name);

            var OrgUnitManagement = userManagement.Children.FirstOrDefault(p => p.Name == OrgUnitPermissions.OrgUnit_Node) ??
                             userManagement.CreateChildPermission(OrgUnitPermissions.OrgUnit_Node, OrgUnitPermissions.OrgUnit_Node_Name);

            OrgUnitManagement.CreateChildPermission(OrgUnitPermissions.OrgUnit_Create, OrgUnitPermissions.OrgUnit_Create_Name);
            OrgUnitManagement.CreateChildPermission(OrgUnitPermissions.OrgUnit_Update, OrgUnitPermissions.OrgUnit_Update_Name);
            OrgUnitManagement.CreateChildPermission(OrgUnitPermissions.OrgUnit_Delete, OrgUnitPermissions.OrgUnit_Delete_Name);
        }

        //private static ILocalizableString L(string name)
        //{
        //    return new LocalizableString(name, TemplateConsts.LocalizationSourceName);
        //}
    }
}
