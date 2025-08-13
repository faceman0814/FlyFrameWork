using FlyFramework.Authorizations;
using FlyFramework.FlyFrameworkModules.Permissions;

using System.Linq;


namespace FlyFramework.UserModule.Authorization
{
    /// <summary>
    /// 权限配置都在这里。
    /// 给权限默认设置服务
    /// See <see cref="UserPermissions" /> 
    ///</summary>
    public class UserAuthorizationProvider : AuthorizationProvider
    {

        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            // 在这里配置了FeeType 的权限。
            var pages = context.GetPermissionOrNull(AppPermissions.Pages) ??
                        context.CreatePermission(AppPermissions.Pages, "菜单根节点");

            var userManagement = pages.Children.FirstOrDefault(p => p.Name == UserPermissions.UserManager) ??
                                 pages.CreateChildPermission(UserPermissions.UserManager, UserPermissions.UserManager_Name);

            var user = userManagement.CreateChildPermission(UserPermissions.User_Node, UserPermissions.User_Node_Name);
            user.CreateChildPermission(UserPermissions.User_Create, UserPermissions.User_Create_Name);
        }

        //private static ILocalizableString L(string name)
        //{
        //	return new LocalizableString(name, TemplateConsts.LocalizationSourceName);
        //}
    }

}
