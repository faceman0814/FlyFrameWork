using FlyFramework.ApplicationServices;
using FlyFramework.Authorization;
using FlyFramework.Authorizations;
using FlyFramework.Common;
using FlyFramework.Dtos;
using FlyFramework.Extentions;
using FlyFramework.Extentions.Object;
using FlyFramework.LazyModule.LazyDefinition;
using FlyFramework.OrgUnitModule.DomainService.OrgUnitNodes;
using FlyFramework.PermissionModule.DomainService;
using FlyFramework.PermissionModule.Dtos;
using FlyFramework.Repositories;
using FlyFramework.UserModule.DomainService;
using FlyFramework.UserModule.Dtos;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using System;
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
        private readonly IRoleManager _roleManager;
        private readonly IRepository<UserRole, string> _repository;
        private readonly IOrgUnitNodeManager _orgUnitNodeManager;
        private readonly ICommonAppService _commonService;
        private readonly IPermissionManager _permissionManager;

        public UserAppService(IFlyFrameworkLazy flyFrameworkLazy, ICommonAppService commonAppService, IRepository<UserRole, string> repository)
        {
            _userManager = flyFrameworkLazy.LazyGetRequiredService<IUserManager>().Value;
            _orgUnitNodeManager = flyFrameworkLazy.LazyGetRequiredService<IOrgUnitNodeManager>().Value;
            _roleManager = flyFrameworkLazy.LazyGetRequiredService<IRoleManager>().Value;
            _permissionManager = flyFrameworkLazy.LazyGetRequiredService<IPermissionManager>().Value;
            _commonService = commonAppService;
            _repository = repository;
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
                await Update(input);
            }
            else
            {
                await Create(input);
            }
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public async Task<UserDto> GetForEdit(EntityDto<string> input)
        {
            var entity = await _userManager.FindById(input.Id);
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
                        join org in _orgUnitNodeManager.QueryAsNoTracking on user.OrgUnitNodeId equals org.Id into oGroup
                        from org in oGroup.DefaultIfEmpty()
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

            var userIds = datas
                .Select(t => t.Id)
                .ToList();

            var roleQuery = from ur in _repository.GetAll()
                            join r in _roleManager.QueryAsNoTracking on ur.RoleId equals r.Id
                            where userIds.Contains(ur.UserId)
                            select new
                            {
                                ur.UserId,
                                r.DisplayName
                            };

            var roles = await roleQuery.ToListAsync();
            foreach (var item in datas)
            {
                item.RoleName = roles.Where(t => t.UserId == item.Id).Select(t => t.DisplayName).ToList();
            }
            res.datas = new PagedResultDto<UserListDto>(await query.CountAsync(), resDatas);
            return res;
        }

        /// <summary>
        /// 批量分配角色
        /// </summary>
        /// <param name="input"></param>
        public async Task AssignRole(AssignRoleInput input)
        {
            //删除旧用户角色关系
            await _repository.DeleteAsync(t => input.UserIds.Contains(t.UserId));

            //遍历添加新用户角色关系
            foreach (var user in input.UserIds)
            {
                foreach (var role in input.RoleIds)
                {
                    await _repository.InsertAsync(new UserRole(user, role));
                }
            }
        }

        /// <summary>
        /// 获取所有权限
        /// </summary>
        /// <returns></returns>
        public async Task<List<PermissionDto>> GetAllPermission()
        {
            return await _permissionManager.GetAllPermission();
        }

        #region 私有方法
        private async Task Create(CreateOrUpdateUserInput input)
        {
            var user = ObjectMapper.Map<User>(input.User);
            await _userManager.CreateUserAsync(user);
        }

        private async Task Update(CreateOrUpdateUserInput input)
        {
            var user = await _userManager.FindById(input.User.Id);
            ObjectMapper.Map(input.User, user);
            await _userManager.Update(user);
        }

        #endregion
    }
}
