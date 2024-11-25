using FaceMan.DynamicWebAPI;

using FlyFramework.Dtos;
using FlyFramework.OrgUnitModule.OrgUnitNodes.Dtos;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlyFramework.OrgUnitModule.OrgUnitNodes
{
    public interface IOrgUnitNodeAppService : IApplicationService
    {
        Task<List<OrgUnitNodeListDto>> GetTree(GetOrgUnitNodesInput input);

        Task<OrgUnitNodeForEditOutput> GetForEdit(EntityDto<string> input);

        Task CreateOrUpdate(CreateOrUpdateOrgUnitNodeInput input);

        Task Delete(EntityDto<string> input);
    }
}
