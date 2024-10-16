using FlyFramework.Domains;
using FlyFramework.OrganizationalUnitModule;
using FlyFramework.UserModule;
using FlyFramework.UserModule.DomainService;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.OrgUnitModule.DomainService.OrgUnits
{
    public class OrgUnitManager : GuidDomainService<OrgUnit>, IOrgUnitManager
    {
        public OrgUnitManager(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public Task<OrgUnit> FindByNameAsync(string name)
        {
            return QueryAsNoTracking.FirstOrDefaultAsync(t => t.Name == name);
        }

        public override IQueryable<OrgUnit> GetIncludeQuery()
        {
            throw new NotImplementedException();
        }

        public override Task ValidateOnCreateOrUpdate(OrgUnit entity)
        {
            return Task.CompletedTask;
        }

        public override Task ValidateOnDelete(OrgUnit entity)
        {
            return Task.CompletedTask;
        }
    }
}
