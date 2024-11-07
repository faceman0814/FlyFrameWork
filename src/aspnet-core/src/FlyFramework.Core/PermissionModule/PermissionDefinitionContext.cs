using FlyFramework.ErrorExceptions;
using FlyFramework.Extentions;
using FlyFramework.LazyModule.LazyDefinition;
using FlyFramework.PermissionModule.DomainService;
using FlyFramework.Repositories;
using FlyFramework.UserModule.DomainService;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.PermissionModule
{
    public class PermissionDefinitionContext : IPermissionDefinitionContext
    {
        readonly IRepository<Permission, string> _permissionManager;

        public PermissionDefinitionContext(IServiceProvider serviceProvider)
        {
            _permissionManager = serviceProvider.GetService<IRepository<Permission, string>>();
        }

        public Permission CreatePermission(string name, string displayName = null, Dictionary<string, object> properties = null)
        {
            Check.NotNull(name, nameof(name));
            var permission = new Permission(name, displayName, properties);
            _permissionManager.Insert(permission);
            return permission;
        }

        public Permission GetPermissionOrNull(string name)
        {
            Check.NotNull(name, nameof(name));
            return _permissionManager.GetAll().FirstOrDefault(t => t.Name == name);
        }

        public void RemovePermission(string name)
        {
            Check.NotNull(name, nameof(name));
            var permission = GetPermissionOrNull(name);
            if (permission == null)
            {
                throw new EntityNotFoundException(typeof(Permission), name);
            }
            _permissionManager.Delete(permission.Id);
        }
    }
}
