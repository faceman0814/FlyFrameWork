using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace FlyFramework.Authorizations
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class FlyFrameworkAuthorizationAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public string RequiredPermission { get; }


        public FlyFrameworkAuthorizationAttribute(string requiredPermission)
        {
            RequiredPermission = requiredPermission;
        }
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // 这里假设你已经有了一种方法来获取当前用户的权限信息
            var currentUserPermissions = await GetCurrentUsersAuthority(context);

            if (!currentUserPermissions.Contains(RequiredPermission))
            {
                // 用户没有权限访问，返回403，并附带提示信息
                context.Result = new ForbidResult(context.RouteData.Values["action"].ToString());
            }
        }

        private async Task<List<string>> GetCurrentUsersAuthority(AuthorizationFilterContext context)
        {
            // 模拟从某处获取用户权限的过程
            return await Task.FromResult(new List<string> { "test", "example" });
        }
    }
}
