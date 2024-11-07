using FlyFramework.Extentions.Object;
using FlyFramework.PermissionModule;
using FlyFramework.UserModule;
using FlyFramework.UserModule.Authority;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// 创建角色关联的权限
        /// </summary>
        /// <param name="roleId">角色id</param>
        /// <param name="authorizationProviders">权限定义</param>
        public void CreateRolePermissions(string roleId)
        {
            //todo 权限定义
            //var isExists = _context.Permission.Any(t => t.Name.Contains(UserAuthority.User_Node));
            //if (!isExists)
            //{
            //    var perssions = new List<Permission>();
            //    var node = new Permission(UserAuthority.User_Node, "用户结点")
            //    {
            //        Id = Guid.NewGuid().ToString("N")
            //    };
            //    node.CreateChildPermission(UserAuthority.User_Create, "创建用户");
            //    node.CreateChildPermission(UserAuthority.User_Delete, "删除用户");
            //    node.CreateChildPermission(UserAuthority.User_Update, "更新用户");
            //    _context.Permission.AddRange(perssions);
            //}
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
                var userRole = new UserRole(_tenantId, user.Id, roleId);
                _context.UserRole.Add(userRole);
            }
        }
    }
}
