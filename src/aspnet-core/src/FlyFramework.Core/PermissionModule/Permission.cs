using FlyFramework.Entities;

using Microsoft.AspNet.SignalR.Hubs;

using ServiceStack;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.PermissionModule
{
    public class Permission : Entity<string>
    {
        /// <summary>
        /// 权限Key
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 权限名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 父权限
        /// </summary>
        public Permission Parent { get; set; }
        /// <summary>
        /// 子权限
        /// </summary>
        public IReadOnlyList<Permission> Children => _children.ToImmutableList();
        private readonly List<Permission> _children;

        public Permission()
        {

        }
        public Permission(
           string name,
           string displayName = null,
           Dictionary<string, object> properties = null)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            
            Name = name;
            DisplayName = displayName;

            _children = new List<Permission>();
        }

        public Permission CreateChildPermission(
           string name,
           string displayName = null,
           Dictionary<string, object> properties = null)
        {
            var permission = new Permission(name, displayName, properties) { Parent = this };
            _children.Add(permission);
            return permission;
        }

        public void RemoveChildPermission(string name)
        {
            _children.RemoveAll(p => p.Name == name);
        }

        public override string ToString()
        {
            return string.Format("[Permission: {0}]", Name);
        }
    }
}
