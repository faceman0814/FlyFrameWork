using AngleSharp.Html.Dom.Events;

using FaceMan.DynamicWebAPI;

using FlyFramework.ApplicationServices;
using FlyFramework.Attributes;
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
        public async Task CreateUser(CreateOrUpdateUserParam input)
        {
            var user = ObjectMapper.Map<User>(input.Entity);
            await _userManager.CreateUserAsync(user);
        }

        [FlyFrameworkAuthorization("test2")]
        public async Task UpdateUser(CreateOrUpdateUserParam input)
        {
            var user = await _userManager.FindByNameAsync(input.Entity.UserName);
            ObjectMapper.Map(input.Entity, user);
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
            var findkey = $"GetColumnList_{typeof(T).FullName}";
            var res = await _cacheManager.GetCacheAsync<List<ColumnDto>>(findkey);

            if (res == null)
            {
                var properties = typeof(T).GetProperties();
                var indexedProperties = properties
                    .Select((p, index) =>
                    {
                        var sort = p.GetCustomAttribute<SortAttribute>();
                        return new
                        {
                            Property = p,
                            ColumnAttribute = p.GetCustomAttribute<ColumnAttribute>(),
                            Index = sort != null ? sort.Order : index // 记录原始顺序
                        };
                    })
                    .Where(x => x.ColumnAttribute != null)
                    .ToList();

                // 排序逻辑：
                // 1. 先按 SortAttribute.Order 升序排列
                // 2. 如果未配置 SortAttribute，则按原始声明顺序排列
                res = indexedProperties
                    .OrderBy(x => x.Index) // 确保未标记 SortAttribute 的属性保持原始顺序
                    .Select(x =>
                    {
                        var propName = ToCamelCase(x.Property.Name);
                        return new ColumnDto
                        {
                            label = x.ColumnAttribute.Name ?? x.Property.Name,
                            prop = propName,
                            slot = propName
                        };
                    })
                    .ToList();

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
        /// <summary>
        /// 操作
        /// </summary>
        [Column("操作")]
        [Sort(99)]
        public String Opertion { get; set; }
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
