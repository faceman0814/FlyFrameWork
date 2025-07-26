using FlyFramework.ApplicationServices;
using FlyFramework.Attributes;
using FlyFramework.Dtos;
using FlyFramework.UserModule.Dtos;
using FlyFramework.Utilities.Redis;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Common
{
    public class CommonAppService: ApplicationService,ICommonAppService
    {
        private readonly ICacheManager _cacheManager;
        public CommonAppService(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
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
}
