using FlyFramework.Authorizations;
using FlyFramework.PermissionModule;

using ServiceStack.Auth;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.UserModule.Authorization
{
    public class UserAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var pages = context.GetPermissionOrNull(AppPermissions.Pages) ??
                         context.CreatePermission(AppPermissions.Pages, "页面");

            var userManagement = pages.Children.FirstOrDefault(p => p.Name == UserPermissions.User_Node) ??
                             pages.CreateChildPermission(UserPermissions.User_Node, UserPermissions.User_Node_Name);

            userManagement.CreateChildPermission(UserPermissions.User_Create, UserPermissions.User_Create_Name);
            userManagement.CreateChildPermission(UserPermissions.User_Update, UserPermissions.User_Update_Name);
            userManagement.CreateChildPermission(UserPermissions.User_Delete, UserPermissions.User_Delete_Name);
        }

        //private static ILocalizableString L(string name)
        //{
        //    return new LocalizableString(name, TemplateConsts.LocalizationSourceName);
        //}
    }
}
