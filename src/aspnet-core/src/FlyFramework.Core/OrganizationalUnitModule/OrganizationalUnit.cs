using FlyFramework.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.OrganizationalUnitModule
{
    public class OrganizationalUnit : FullAuditedEntity<string>, IMayHaveTenant
    {
        public string TenantId { get; set; }
    }
}
