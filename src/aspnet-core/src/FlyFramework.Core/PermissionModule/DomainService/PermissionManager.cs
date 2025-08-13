using FlyFramework.Domains;
using FlyFramework.OrgUnitModule;
using FlyFramework.PermissionModule.Dtos;
using FlyFramework.Repositories;
using FlyFramework.UserModule;
using FlyFramework.UserModule.DomainService;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyFramework.PermissionModule.DomainService
{
    public class PermissionManager : GuidDomainService<PermissionSetting>, IPermissionManager
    {
        readonly IRolePermissionManager _rolePermissionManager;
        public PermissionManager(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _rolePermissionManager = serviceProvider.GetService<IRolePermissionManager>();
        }

        public override IQueryable<PermissionSetting> GetIncludeQuery()
        {
            throw new NotImplementedException();
        }

        public override Task ValidateOnDelete(PermissionSetting entity)
        {
            return Task.CompletedTask;
        }

        public override Task ValidateOnCreateOrUpdate(PermissionSetting entity)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 获取所有权限
        /// </summary>
        /// <returns></returns>
        public async Task<List<PermissionDto>> GetAllPermission()
        {
            var allPermissions = await this.QueryAsNoTracking
                .Select(t => new PermissionDto
                {
                    Id = t.Id,
                    Name = t.Name,
                })
                .ToListAsync();

            // 构建权限树
            return BuildPermissionTree(allPermissions);
        }

        public async Task AssignPermission(AssignPermissionInput input)
        {
            switch (input.Type)
            {
                case AssignPermissionEnum.OrgUnit:
                case AssignPermissionEnum.User:
                    break;
                case AssignPermissionEnum.Role:
                    foreach (var item in input.PermissionIds)
                    {
                        await _rolePermissionManager.Create(new RolePermission()
                        {
                            RoleId = input.Id,
                            PermissionId = item
                        });
                    }
                    break;
            }
        }

        #region 私有方法

        private List<PermissionDto> BuildPermissionTree(List<PermissionDto> allPermissions)
        {
            // 获取所有根节点（ParentId为null或空字符串的节点）
            var rootPermissions = allPermissions.Where(p => string.IsNullOrEmpty(p.ParentId)).ToList();

            foreach (var rootPermission in rootPermissions)
            {
                // 递归构建子树
                rootPermission.Children = GetChildPermissions(rootPermission.Id, allPermissions);
            }

            return rootPermissions;
        }

        private List<PermissionDto> GetChildPermissions(string parentId, List<PermissionDto> allPermissions)
        {
            // 获取直接子节点
            var children = allPermissions.Where(p => p.ParentId == parentId).ToList();

            // 递归获取每个子节点的子节点
            foreach (var child in children)
            {
                child.Children = GetChildPermissions(child.Id, allPermissions);
            }

            return children;
        }

        private List<PermissionDto> BuildPermissionTreeSafe(List<PermissionDto> allPermissions)
        {
            var rootPermissions = allPermissions.Where(p => string.IsNullOrEmpty(p.ParentId)).ToList();
            var visited = new HashSet<string>();

            foreach (var rootPermission in rootPermissions)
            {
                rootPermission.Children = GetChildPermissionsSafe(rootPermission.Id, allPermissions, visited);
            }

            return rootPermissions;
        }

        private List<PermissionDto> GetChildPermissionsSafe(string parentId, List<PermissionDto> allPermissions, HashSet<string> visited)
        {
            if (visited.Contains(parentId))
            {
                // 检测到循环引用
                return new List<PermissionDto>();
            }

            visited.Add(parentId);

            var children = allPermissions.Where(p => p.ParentId == parentId).ToList();

            foreach (var child in children)
            {
                child.Children = GetChildPermissionsSafe(child.Id, allPermissions, visited);
            }

            visited.Remove(parentId);

            return children;
        }

        #endregion
    }
}
