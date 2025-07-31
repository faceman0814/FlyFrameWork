using FlyFramework.ApplicationServices;
using FlyFramework.Authorizations;
using FlyFramework.Common;
using FlyFramework.Dtos;
using FlyFramework.Extentions;
using FlyFramework.Extentions.Object;
using FlyFramework.LazyModule.LazyDefinition;
using FlyFramework.OrgUnitModule.DomainService.OrgUnitNodes;
using FlyFramework.RoleModule.Dtos;
using FlyFramework.UserModule;
using FlyFramework.UserModule.DomainService;
using FlyFramework.UserModule.Dtos;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace FlyFramework.RoleModule
{
    public class RoleAppService : ApplicationService, IRoleAppService
    {
        private readonly IRoleManager _roleManager;
        private readonly ICommonAppService _commonService;
        public RoleAppService(IFlyFrameworkLazy flyFrameworkLazy, ICommonAppService commonAppService)
        {
            _roleManager = flyFrameworkLazy.LazyGetRequiredService<IRoleManager>().Value;
            _commonService = commonAppService;
        }

        /// <summary>
        /// 获取角色列表分页数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<GetPagedResult<RoleListDto>> GetPaged(GetRolesInput input)
        {
            var res = new GetPagedResult<RoleListDto>()
            {
                columns = await _commonService.GetColumnList<RoleListDto>()
            };

            var datas = await _roleManager.QueryAsNoTracking
                .PageBy(input)
                .ToListAsync();

            var resDatas = ObjectMapper.Map<List<RoleListDto>>(datas);

            res.datas = new PagedResultDto<RoleListDto>(await _roleManager.QueryAsNoTracking.CountAsync(), resDatas);
            return res;
        }

        /// <summary>
        /// 创建或更新角色
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreateOrUpdate(CreateOrUpdateRoleInput input)
        {
            if (input.Role.Id.HasValue())
            {
                await Update(input);
            }
            else
            {
                await Create(input);
            }
        }

        /// <summary>
        /// 获取角色信息
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public async Task<RoleDto> GetForEdit(EntityDto<string> input)
        {
            var entity = await _roleManager.FindById(input.Id);
            var role = ObjectMapper.Map<RoleDto>(entity);
            return role;
        }

        #region 私有方法

        private async Task Create(CreateOrUpdateRoleInput input)
        {
            var entity = ObjectMapper.Map<Role>(input.Role);
            entity.NormalizedName = entity.Name.ToUpper();
            await _roleManager.Create(entity);
        }

        private async Task Update(CreateOrUpdateRoleInput input)
        {
            var entity = await _roleManager.FindById(input.Role.Id);
            ObjectMapper.Map(input.Role, entity);
            await _roleManager.Update(entity);
        }
        #endregion
    }
}
