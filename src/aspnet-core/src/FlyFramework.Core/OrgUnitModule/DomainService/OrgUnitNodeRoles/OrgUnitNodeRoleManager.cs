using FlyFramework.Domains;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace FlyFramework.OrgUnitModule.DomainService.OrgUnitNodeRoles
{
    public class OrgUnitNodeRoleManager : GuidDomainService<OrgUnitNodeRole>, IOrgUnitNodeRoleManager
    {
        public OrgUnitNodeRoleManager(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public Task<OrgUnitNodeRole> FindByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public override IQueryable<OrgUnitNodeRole> GetIncludeQuery()
        {
            throw new NotImplementedException();
        }

        public override Task ValidateOnCreateOrUpdate(OrgUnitNodeRole entity)
        {
            return Task.CompletedTask;
        }

        public override Task ValidateOnDelete(OrgUnitNodeRole entity)
        {
            return Task.CompletedTask;
        }
    }
}
