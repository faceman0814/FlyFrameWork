using FlyFramework.Domains;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace FlyFramework.UserModule.DomainService
{
    public class RoleManager : GuidDomainService<Role>, IRoleManager
    {
        public RoleManager(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override IQueryable<Role> GetIncludeQuery()
        {
            return this.Query;
        }

        public override Task ValidateOnCreateOrUpdate(Role entity)
        {
            return Task.CompletedTask;
        }

        public override Task ValidateOnDelete(Role entity)
        {
            return Task.CompletedTask;
        }
    }
}
