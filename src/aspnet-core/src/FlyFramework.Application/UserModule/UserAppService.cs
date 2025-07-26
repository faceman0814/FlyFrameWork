using AngleSharp.Html.Dom.Events;

using FaceMan.DynamicWebAPI;

using FlyFramework.ApplicationServices;
using FlyFramework.Attributes;
using FlyFramework.Authorizations;
using FlyFramework.Common;
using FlyFramework.Dtos;
using FlyFramework.Extentions;
using FlyFramework.LazyModule.LazyDefinition;
using FlyFramework.UserModule.DomainService;
using FlyFramework.UserModule.Dtos;
using FlyFramework.Utilities.Redis;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ServiceStack;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Threading.Tasks;
namespace FlyFramework.UserModule
{
    //[Authorize]
    //[DynamicWebApi]
    public class UserAppService : ApplicationService, IUserAppService
    {
        private readonly IUserManager _userManager;
        private readonly ICommonAppService _commonService;

        public UserAppService(IFlyFrameworkLazy flyFrameworkLazy, ICommonAppService commonAppService)
        {
            _userManager = flyFrameworkLazy.LazyGetRequiredService<IUserManager>().Value;
            _commonService = commonAppService;
        }

        [FlyFrameworkAuthorization("test")]
        public async Task CreateOrUpdateUser(CreateOrUpdateUserParam input)
        {
            if (!string.IsNullOrWhiteSpace(input.Entity.Id))
            {
                await UpdateUser(input);
            }
            else
            {
                await CreateUser(input);
            }
        }

        private async Task CreateUser(CreateOrUpdateUserParam input)
        {
            var user = ObjectMapper.Map<User>(input.Entity);
            await _userManager.CreateUserAsync(user);
        }

        private async Task UpdateUser(CreateOrUpdateUserParam input)
        {
            var user = await _userManager.FindByNameAsync(input.Entity.UserName);
            ObjectMapper.Map(input.Entity, user);
            await _userManager.Update(user);
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

            var query = _userManager.QueryAsNoTracking;

            var datas = await query.PageBy(input).ToListAsync();

            var resDatas = ObjectMapper.Map<List<UserListDto>>(datas);

            res.datas = new PagedResultDto<UserListDto>(await query.CountAsync(), resDatas);
            return res;
        }
    }
}
