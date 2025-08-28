using FaceMan.DynamicWebAPI;

using FlyFramework.Dtos;
using FlyFramework.RoleModule.Dtos;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlyFramework.RoleModule
{
    public interface IRoleAppService : IApplicationService
    {
        Task<PagedResultDto<RoleListDto>> GetPaged(GetRolesInput input);

        Task CreateOrUpdate(CreateOrUpdateRoleInput input);

        Task<RoleDto> GetForEdit(EntityDto<string> input);

        Task<List<DropDownListDto>> GetDropDownList(GetDropDownListInput input);
    }
}
