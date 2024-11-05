using FlyFramework.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.PermissionModule
{
    public class Permission : FullAuditedEntity<string>, IMayHaveTenant
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public string Value { get; set; }
        public string TenantId { get; set; }

        public Permission( string roleId, string value)
        {
            Id = Guid.NewGuid().ToString("N");
            RoleId = roleId;
            Value = value;
        }
    }
}
