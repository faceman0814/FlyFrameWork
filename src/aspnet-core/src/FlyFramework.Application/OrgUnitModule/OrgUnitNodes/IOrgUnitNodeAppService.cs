using FlyFramework.ApplicationServices;
using FlyFramework.Dtos;
using FlyFramework.OrgUnitModule.OrgUnitNodes.Dtos;
using FlyFramework.OrgUnitModule.OrgUnits.Dtos;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.OrgUnitModule.OrgUnitNodes
{
    public interface IOrgUnitNodeAppService : IApplicationService
    {
        Task<List<OrgUnitNodeListDto>> GetTree(GetOrgUnitsInput input);

        Task<OrgUnitNodeForEditOutput> GetForEdit(EntityDto<string> input);

        Task CreateOrUpdate(CreateOrUpdateOrgUnitNodeInput input);

        Task Delete(EntityDto<string> input);
    }
}
