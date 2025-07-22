using FlyFramework.Domains;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace FlyFramework.PermissionModule.DomainService
{
    public class PermissionManager : GuidDomainService<Permission>, IPermissionManager
    {
        public PermissionManager(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override IQueryable<Permission> GetIncludeQuery()
        {
            throw new NotImplementedException();
        }

        public override Task ValidateOnDelete(Permission entity)
        {
            return Task.CompletedTask;
        }

        public override Task ValidateOnCreateOrUpdate(Permission entity)
        {
            return Task.CompletedTask;
        }
    }
}
