using FaceMan.DynamicWebAPI;

using FlyFramework.ApplicationServices;
using FlyFramework.Authorizations;
using FlyFramework.Dtos;
using FlyFramework.Extentions;
using FlyFramework.LazyModule.LazyDefinition;
using FlyFramework.UserModule.DomainService;
using FlyFramework.UserModule.Dtos;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
namespace FlyFramework.UserModule
{
    //[Authorize]
    //[DynamicWebApi]
    public class UserAppService : ApplicationService, IUserAppService
    {
        private readonly IUserManager _userManager;

        public UserAppService(IServiceProvider serviceProvider, IFlyFrameworkLazy flyFrameworkLazy, IUserManager userManager)
        {
            _userManager = flyFrameworkLazy.LazyGetRequiredService<IUserManager>().Value;
        }

        [FlyFrameworkAuthorization("test")]
        public async Task CreateUser(UserDto input)
        {
            var user = ObjectMapper.Map<User>(input);
            await _userManager.CreateUserAsync(user);
        }

        [FlyFrameworkAuthorization("test2")]
        public async Task UpdateUser(UserDto input)
        {
            var user = await _userManager.FindByNameAsync(input.UserName);
            ObjectMapper.Map(input, user);
            await _userManager.Update(user);
        }

        /// <summary>
        /// 获取用户列表分页数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<PagedResultDto<UserListDto>> GetPaged(GetUsersInput input)
        {
            //var res = new GetUserResult()
            //{
            //    columns = GetColumnList<UserListDto>()
            //};

            var query = _userManager.QueryAsNoTracking
                .Select(t => new UserListDto
                {
                    Id = t.Id,
                    UserName ="test",
                    Email ="1002784867@qq.com",
                    CreationTime = t.CreationTime
                });

            var datas = await query.PageBy(input).ToListAsync();

            return new PagedResultDto<UserListDto>(await query.CountAsync(), datas);
        }

        public List<ColumnDto> GetUserColumnList()
        {
            //获取用户列表的列信息
            return GetColumnList<UserListDto>();
        }
        /// <summary>
        /// 获取用户列表的列信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<ColumnDto> GetColumnList<T>() where T : class
        {
            //使用反射获取标注了Column属性的字段列信息
            var properties = typeof(T).GetProperties()
                .Where(p => p.GetCustomAttributes(typeof(ColumnAttribute), false).Any())
                .Select(p => new ColumnDto
                {
                    label = p.GetCustomAttributes(typeof(ColumnAttribute), false)
                        .Cast<ColumnAttribute>().FirstOrDefault()?.Name ?? p.Name,
                    prop = p.Name
                }).ToList();

            return properties;
        }
    }

    public class ColumnDto
    {
        public string label { get; set; }
        public string prop { get; set; }
    }

    public class UserListDto : UserDto
    {

    }

    public class GetUsersInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        /// <summary>
		/// 正常化排序使用
		/// </summary>
		public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime desc";
            }
        }
    }

}
