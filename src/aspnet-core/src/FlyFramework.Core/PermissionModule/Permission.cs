using FlyFramework.Entities;

using Microsoft.AspNet.SignalR.Hubs;

using ServiceStack;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.PermissionModule
{
    /// <summary>
    /// 权限表
    /// </summary>
    public class Permission : Entity<string>
    {
        private readonly List<Permission> _children;

        public Permission(string key, string displayName)
        {
            Key = key;
            DisplayName = displayName;
            Id = Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// 权限Key
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 权限名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 权限类型
        /// </summary>
        public PermissionType Type { get; set; }

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
                Parent = this
            };
            _children.Add(permission);
            return permission;
        }

        public enum PermissionType
        {
            [Description("操作权限")]
            Operation,

            [Description("数据权限")]
            Data
        }
    }
}
