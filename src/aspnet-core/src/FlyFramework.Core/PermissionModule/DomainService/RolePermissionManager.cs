using FlyFramework.Domains;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace FlyFramework.PermissionModule.DomainService
{


    public class RolePermissionManager : GuidDomainService<RolePermission>, IRolePermissionManager
    {

        public RolePermissionManager(IServiceProvider serviceProvider) :
                base(serviceProvider)
        {
        }

        public override IQueryable<RolePermission> GetIncludeQuery()
        {
            throw new NotImplementedException();
        }

        public override Task ValidateOnCreateOrUpdate(RolePermission entity)
        {
            return Task.CompletedTask;
        }

        public override Task ValidateOnDelete(RolePermission entity)
        {
            return Task.CompletedTask;
        }
    }
}
