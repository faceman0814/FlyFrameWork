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
    public class OrgUnitNodeGrantedManager : GuidDomainService<OrgUnitNodeGranted>, IOrgUnitNodeGrantedManager
    {
        public OrgUnitNodeGrantedManager(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public Task<OrgUnitNodeGranted> FindByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public override IQueryable<OrgUnitNodeGranted> GetIncludeQuery()
        {
            throw new NotImplementedException();
        }

        public override Task ValidateOnCreateOrUpdate(OrgUnitNodeGranted entity)
        {
            return Task.CompletedTask;
        }

        public override Task ValidateOnDelete(OrgUnitNodeGranted entity)
        {
            return Task.CompletedTask;
        }
    }
}
