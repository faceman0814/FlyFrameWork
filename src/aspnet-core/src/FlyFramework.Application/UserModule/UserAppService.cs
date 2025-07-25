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
            var findkey = $"UserColumnList_{typeof(T).FullName}";
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
        /// 显示的标题
        /// </summary>
        public string label { get; set; }
        /// <summary>
        /// 字段名称
        /// </summary>
        public string prop { get; set; }
        /// <summary>
        /// 指定插槽
        /// </summary>
        public string slot { get; set; }

        /// <summary>
        ///  对应列的类型，
        ///  如果设置了 `selection` 则显示多选框；
        ///  如果设置了 `index` 则显示该行的索引（从 `1` 开始计算）；
        ///  如果设置了 `expand` 则显示为一个可展开的按钮 */
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 如果设置了 `type=index`，可以通过传递 `index` 属性来自定义索引
        /// </summary>
        public int index { get; set; }

        /// <summary>
        /// `column` 的 `key`， 如果需要使用 `filter-change` 事件，则需要此属性标识是哪个 `column` 的筛选条件
        /// </summary>
        public string columnKey { get; set; }

        /// <summary>
        /// 对应列的宽度
        /// </summary>
        public string width { get; set; }

        /// <summary>
        /// 对应列的最小宽度
        /// </summary>
        public string minWidth { get; set; }

        /// <summary>
        /// 列是否固定在左侧或者右侧。`true` 表示固定在左侧
        /// </summary>
        public string Fixed { get; set; }

        /// <summary>
        /// 对应列是否可以排序
        /// 如果设置为 `'custom'`，则代表用户希望远程排序，需要监听 `Table` 的 `sort-change `事件
        /// 默认值为 `false` 
        /// </summary>
        public string sortable { get; set; } = "custom";

        /// <summary>
        /// 对应列是否可以通过拖动改变宽度（需要在 `el-table` 上设置 `border` 属性为真），默认值为 `true`
        /// </summary>
        public bool resizable { get; set; } = true;

        /// <summary>
        /// 列标题 `Label` 区域渲染使用的 `Function` 
        /// </summary>
        public string renderHeader { get; set; }

        /// <summary>
        /// 指定数据按照哪个属性进行排序，仅当 `sortable` 设置为 `true` 的时候有效。应该如同 `Array.sort` 那样返回一个 `Number`
        /// </summary>
        public int sortMethod { get; set; }

        /// <summary>
        /// 指定数据按照哪个属性进行排序，仅当 `sortable` 设置为 `true` 且没有设置 `sort-method` 的时候有效。
        /// 如果 `sort-by` 为数组，则先按照第 `1` 个属性排序，
        /// 如果第 `1` 个相等，再按照第 `2` 个排序，以此类推 
        /// </summary>
        public List<string> sortBy { get; set; }

        /// <summary>
        /// 数据在排序时所使用排序策略的轮转顺序，仅当 `sortable` 为 `true` 时有效。
        /// 需传入一个数组，随着用户点击表头，该列依次按照数组中元素的顺序进行排序，
        /// 默认值为 `['ascending', 'descending', null]` 
        /// </summary>
        public List<string> sortOrders { get; set; } = ["ascending", "descending", null];

        /// <summary>
        /// 用来格式化内容的函数，仅对当前列有效。
        /// </summary>
        public string formatter { get; set; }

        /// <summary>
        ///  当内容过长被隐藏时显示 `tooltip`，默认值为 `false`
        /// </summary>
        public bool showOverflowTooltip { get; set; }

        /// <summary>
        /// 对齐方式，可选值为 `left`、`center`、`right`
        /// </summary>
        public string align { get; set; } = "left";

        /// <summary>
        /// 表头对齐方式，若不设置该项，则使用表格的对齐方式
        /// </summary>
        public string headerAlign { get; set; } = "left";

        /// <summary>
        /// 列的 `className`
        /// </summary>
        public string className { get; set; }

        /// <summary>
        /// 当前列标题的自定义类名
        /// </summary>
        public string labelClassName { get; set; }

        /// <summary>
        /// 仅对 `type=selection` 的列有效，类型为 `Function`，`Function` 的返回值用来决定这一行的 `CheckBox` 是否可以勾选 
        /// </summary>
        public bool selectable { get; set; }

        /// <summary>
        /// 仅对 `type=selection` 的列有效，请注意，需指定 `row-key` 来让这个功能生效，默认值为 `false`
        /// </summary>
        public string reserveSelection { get; set; }

        /// <summary>
        /// 数据过滤的选项，数组格式，数组中的元素需要有 `text` 和 `value` 属性。
        /// 数组中的每个元素都需要有 `text` 和 `value` 属性 
        /// </summary>
        public List<ColumnFilters> filters { get; set; }

        /// <summary>
        /// 过滤弹出框的定位
        /// "top-start" |
        /// "top-end" | 
        /// "top" |
        /// "bottom-start" | 
        /// "bottom-end" | 
        /// "bottom" | 
        /// "left-start" | 
        /// "left-end" | 
        /// "left" | 
        /// "right-start" | 
        /// "right-end" | 
        /// "right";
        /// </summary>
        public string filterPlacement { get; set; }

        /// <summary>
        /// 过滤弹出框的 `className`
        /// </summary>
        public string filterClassName { get; set; }

        /// <summary>
        /// 数据过滤的选项是否多选，默认值为 `true`
        /// </summary>
        public boolean filterMultiple { get; set; } = true;

        ///// <summary>
        ///// 数据过滤使用的方法，如果是多选的筛选项，对每一条数据会执行多次，任意一次返回 `true` 就会显示
        ///// </summary>
        // string filterMethod { get; set; }


        /// <summary>
        /// 选中的数据过滤项，如果需要自定义表头过滤的渲染方式，可能会需要此属性
        /// </summary>
        public List<object> filteredValue { get; set; }
    }

    public class ColumnFilters
    {
        public string text { get; set; }
        public string value { get; set; }
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
