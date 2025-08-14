using FlyFramework.Entities;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace FlyFramework.PermissionModule
{
    /// <summary>
    /// 权限表
    /// </summary>
    public class Permission : Entity<string>
    {
        private readonly List<Permission> _children = [];

        public Permission(string name, string displayName)
        {
            Name = name;
            DisplayName = displayName;
            Id = Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// 权限名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 权限名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 父节点
        /// </summary>
        public string ParentId { get; set; }

        public Permission Parent { get; private set; }

        public IReadOnlyList<Permission> Children => _children.ToImmutableList();

        public Permission CreateChildPermission(string name, string displayName)
        {
            var permission = new Permission(name, displayName)
            {
                Parent = this,
                ParentId = this.Id
            };
            _children.Add(permission);
            return permission;
        }
    }
}
