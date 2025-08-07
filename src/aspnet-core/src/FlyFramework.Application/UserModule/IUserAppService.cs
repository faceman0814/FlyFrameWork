
using FaceMan.DynamicWebAPI;

using FlyFramework.Dtos;
using FlyFramework.UserModule.Dtos;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlyFramework.UserModule
{
    /// <summary>
    /// 用户应用服务接口
    /// </summary>
    public interface IUserAppService : IApplicationService
    {
        Task CreateOrUpdate(CreateOrUpdateUserInput input);

        Task<GetPagedResult<UserListDto>> GetPaged(GetUsersInput input);

        Task<UserDto> GetForEdit(EntityDto<string> input);
    }
}
