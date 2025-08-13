using Microsoft.AspNet.SignalR.Hubs;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;

namespace FlyFramework.FlyFrameworkModules.Permissions
{
    public class Permission
    {
        private readonly List<Permission> _children = [];

        public Permission(string name, string displayName)
        {
            Name = name;
            DisplayName = displayName;
        }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public Permission Parent { get; private set; }
        public Dictionary<string, object> Properties { get; }

        public object this[string key]
        {
            get
            {
                if (Properties.ContainsKey(key))
                {
                    return Properties[key];
                }

                return null;
            }
            set
            {
                Properties[key] = value;
            }
        }

        public IReadOnlyList<Permission> Children => _children.ToImmutableList();

        public Permission CreateChildPermission(string name, string displayName)
        {
            var permission = new Permission(name, displayName)
            {
                Parent = this,
            };
            _children.Add(permission);
            return permission;
        }

        public Permission(string name, string displayName = null, Dictionary<string, object> properties = null)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            Name = name;
            DisplayName = displayName;
            Properties = properties ?? new Dictionary<string, object>();
            _children = [];
        }

        public void RemoveChildPermission(string name)
        {
            _children.RemoveAll((Permission p) => p.Name == name);
        }

        public override string ToString()
        {
            return $"[Permission: {Name}]";
        }
    }
}
