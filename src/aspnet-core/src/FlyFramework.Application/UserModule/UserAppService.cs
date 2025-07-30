using FlyFramework.ApplicationServices;
using FlyFramework.Authorizations;
using FlyFramework.Common;
using FlyFramework.Dtos;
using FlyFramework.Extentions;
using FlyFramework.Extentions.Object;
using FlyFramework.LazyModule.LazyDefinition;
using FlyFramework.OrgUnitModule.DomainService.OrgUnitNodes;
using FlyFramework.UserModule.DomainService;
using FlyFramework.UserModule.Dtos;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace FlyFramework.UserModule
{
    //[Authorize]
    //[DynamicWebApi]
    public class UserAppService : ApplicationService, IUserAppService
    {
        private readonly IUserManager _userManager;
        private readonly IOrgUnitNodeManager _orgUnitNodeManager;
        private readonly ICommonAppService _commonService;

        public UserAppService(IFlyFrameworkLazy flyFrameworkLazy, ICommonAppService commonAppService)
        {
            _userManager = flyFrameworkLazy.LazyGetRequiredService<IUserManager>().Value;
            _orgUnitNodeManager = flyFrameworkLazy.LazyGetRequiredService<IOrgUnitNodeManager>().Value;
            _commonService = commonAppService;
        }

        /// <summary>
        /// 创建或更新用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [FlyFrameworkAuthorization("test")]
        public async Task CreateOrUpdate(CreateOrUpdateUserInput input)
        {
            if (input.User.Id.HasValue())
            {
                await UpdateUser(input);
            }
            else
            {
                await CreateUser(input);
            }
        }


        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public async Task<UserDto> GetUserInfo(string id)
        {
            var entity = await _userManager.FindById(id);
            var user = ObjectMapper.Map<UserDto>(entity);
            return user;
        }

        /// <summary>
        /// 获取用户列表分页数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<GetPagedResult<UserListDto>> GetPaged(GetUsersInput input)
        {
            var res = new GetPagedResult<UserListDto>()
            {
                columns = await _commonService.GetColumnList<UserListDto>()
            };
            var query = from user in _userManager.QueryAsNoTracking
                        join org in _orgUnitNodeManager.QueryAsNoTracking on user.OrgUnitNodeId equals org.Id
                        select new UserListDto()
                        {
                            Id = user.Id,
                            UserName = user.UserName,
                            OrgUnitNodeName = org.Name,
                            Email = user.Email,
                            PhoneNumber = user.PhoneNumber,
                            IsActive = user.IsActive,
                            IsSuperAdmin = user.IsSuperAdmin,
                            CreationTime = user.CreationTime,
                        };

            var datas = await query.PageBy(input).ToListAsync();

            var resDatas = ObjectMapper.Map<List<UserListDto>>(datas);

            res.datas = new PagedResultDto<UserListDto>(await query.CountAsync(), resDatas);
            return res;
        }

        #region 私有方法
        private async Task CreateUser(CreateOrUpdateUserInput input)
        {
            var user = ObjectMapper.Map<User>(input.Entity);
            await _userManager.CreateUserAsync(user);
        }

        private async Task UpdateUser(CreateOrUpdateUserInput input)
        {
            var user = await _userManager.FindByNameAsync(input.Entity.UserName);
            ObjectMapper.Map(input.Entity, user);
            await _userManager.Update(user);
        }

        #endregion
    }
}
