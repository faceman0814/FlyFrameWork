using FlyFramework.Extentions.Object;
using FlyFramework.PermissionModule;
using FlyFramework.RoleModule.Authorization;
using FlyFramework.UserModule;
using FlyFramework.UserModule.Authorization;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using ServiceStack;

using System;
using System.Collections.Generic;
using System.Linq;

namespace FlyFramework.Seed
{
    public class FlyFrameworkCreatorHelper
    {
        string _tenantId;
        FlyFrameworkDbContext _context;


        /// <summary>
        /// 设置上下文
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="context"></param>
        public void SetContext(string tenantId, FlyFrameworkDbContext context)
        {
            _tenantId = tenantId;
            _context = context;
        }


        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="roleName">角色名称</param>
        /// <param name="isDefault">是否为默认角色,默认值: false</param>
        /// <param name="isStatic">是否为系统角色,默认值: true</param>
        /// <returns></returns>
        public string CreateRole(string roleName, string displayName, bool isDefault = false, bool isStatic = true)
        {
            var role = _context.Roles.IgnoreQueryFilters()
                .FirstOrDefault(r => r.TenantId == _tenantId && r.Name == roleName);
            if (role != null)
            {
                return role.Id;
            }


            role = _context.Roles
                   .Add(new Role(_tenantId, roleName, displayName)
                   {
                       IsDefault = isDefault,
                       IsStatic = isStatic
                   }).Entity;
            return role.Id;
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="roleId">角色id</param>
        /// <param name="userName">用户名</param>
        public void CreateUser(string roleId, string userName)
        {
            var user = _context.Users.IgnoreQueryFilters()
                .FirstOrDefault(u => u.TenantId == _tenantId && u.UserName == userName);
            if (user != null)
            {
                return;
            }

            user = new User
            {
                Id = Guid.NewGuid().ToString("N"),
                TenantId = _tenantId,
                UserName = userName,
                FullName = userName,
                Email = $"{userName}@faceman.com",
                NeedToChangeThePassword = false,
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString("N"),
            };
            PasswordHasher<User> ph = new PasswordHasher<User>();
            user.PasswordHash = ph.HashPassword(user, "bb123456");
            user.SetNormalizedNames();
            user = _context.Users.Add(user).Entity;

            // Assign role to user
            if (roleId.HasValue())
            {
                var userRole = new UserRole(user.Id, roleId, _tenantId);
                _context.UserRole.Add(userRole);
            }

            //todo 权限定义
            var isExists = _context.Permission.Any(t => t.Name.Contains(AppPermissions.Pages));
            if (!isExists)
            {
                var ids = new List<string>();
                var page = new Permission(AppPermissions.Pages, "页面");
                ids.Add(page.Id);
                var userManager = page.CreateChildPermission(UserPermissions.UserManager, UserPermissions.UserManager_Name);

                ids.Add(userManager.Id);
                var userAuth = userManager.CreateChildPermission(UserPermissions.User_Node, UserPermissions.User_Node_Name);
                ids.Add(userAuth.Id);

                ids.Add(userAuth.CreateChildPermission(UserPermissions.User_Create, UserPermissions.User_Create_Name).Id);
                ids.Add(userAuth.CreateChildPermission(UserPermissions.User_Update, UserPermissions.User_Update_Name).Id);
                ids.Add(userAuth.CreateChildPermission(UserPermissions.User_Delete, UserPermissions.User_Delete_Name).Id);

                var roleAuth = userManager.CreateChildPermission(RolePermissions.Role_Node, RolePermissions.Role_Node_Name);
                ids.Add(roleAuth.Id);

                ids.Add(roleAuth.CreateChildPermission(RolePermissions.Role_Create, RolePermissions.Role_Create_Name).Id);
                ids.Add(roleAuth.CreateChildPermission(RolePermissions.Role_Update, RolePermissions.Role_Update_Name).Id);
                ids.Add(roleAuth.CreateChildPermission(RolePermissions.Role_Delete, RolePermissions.Role_Delete_Name).Id);

                _context.Add(userManager);
                _context.SaveChanges();

                var rolePermissions = new List<RolePermission>();
                foreach (var item in ids)
                {
                    rolePermissions.Add(new RolePermission()
                    {
                        RoleId = roleId,
                        PermissionId = item,
                        Id = Guid.NewGuid().ToString("N")
                    });
                }

                _context.AddRange(rolePermissions);
            }
        }

    }
}
