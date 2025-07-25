using AngleSharp.Html.Dom.Events;

using FaceMan.DynamicWebAPI;

using FlyFramework.ApplicationServices;
using FlyFramework.Authorizations;
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
        private readonly ICacheManager _cacheManager;

        public UserAppService(IFlyFrameworkLazy flyFrameworkLazy)
        {
            _userManager = flyFrameworkLazy.LazyGetRequiredService<IUserManager>().Value;
            _cacheManager = flyFrameworkLazy.LazyGetRequiredService<ICacheManager>().Value;
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
        public async Task<GetPagedResult<UserListDto>> GetPaged(GetUsersInput input)
        {
            var res = new GetPagedResult<UserListDto>()
            {
                columns = await GetColumnList<UserListDto>()
            };

            var query = _userManager.QueryAsNoTracking;

            var datas = await query.PageBy(input).ToListAsync();

            var resDatas = ObjectMapper.Map<List<UserListDto>>(datas);

            res.datas = new PagedResultDto<UserListDto>(await query.CountAsync(), resDatas);
            return res;
        }

        public async Task<List<ColumnDto>> GetUserColumnList()
        {
            //获取用户列表的列信息
            return await GetColumnList<UserListDto>();
        }
        /// <summary>
        /// 获取用户列表的列信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<List<ColumnDto>> GetColumnList<T>() where T : class
        {
            // 可以根据类型T来缓存结果
            var findkey = $"GetColumnList_{typeof(T).FullName}";
            var res = await _cacheManager.GetCacheAsync<List<ColumnDto>>(findkey);
            if (res == null)
            {
                res = typeof(T).GetProperties()
               .Select(p => (Property: p, Attribute: p.GetCustomAttribute<ColumnAttribute>()))
               .Where(x => x.Attribute != null)
               .Select(x =>
               {
                   var propName = ToCamelCase(x.Property.Name);
                   return new ColumnDto
                   {
                       label = x.Attribute.Name ?? x.Property.Name,
                       prop = propName,
                       slot = propName  // 复用已转换的驼峰命名值
                   };
               })
               .ToList();
                //设置缓存
                await _cacheManager.SetCacheAsync(findkey, res);
            }

            return res;

        }

        // 高效的首字母小写转换方法
        private string ToCamelCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // 处理单个字符的情况
            if (input.Length == 1)
                return char.ToLowerInvariant(input[0]).ToString();

            // 使用Span进行高效内存操作
            return string.Create(input.Length, input, (chars, src) =>
            {
                chars[0] = char.ToLowerInvariant(src[0]);  // 首字母小写
                src.AsSpan(1).CopyTo(chars[1..]);          // 复制剩余字符
            });
        }
    }

    public class ColumnDto
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string label { get; set; }
        /// <summary>
        /// 字段key
        /// </summary>
        public string prop { get; set; }
        /// <summary>
        /// 指定插槽
        /// </summary>
        public string slot { get; set; }
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

    public class GetPagedResult<T> where T : class
    {
        public List<ColumnDto> columns { get; set; }
        public PagedResultDto<T> datas { get; set; }
    }
}
