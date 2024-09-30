using AutoMapper;

using System.Collections.Generic;
using System.Linq;

namespace GCT.MedPro.Application
{
    /// 映射扩展
    /// </summary>
    public static class AutoMapperExtensions
    {
        /// <summary>
        /// 忽略不需要映射的属性。
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="expression"></param>
        /// <param name="ignoreProperties">指定需要忽略的属性名称列表</param>
        /// <returns></returns>
        public static IMappingExpression<TSource, TDestination> IgnoreNullSourceProperties<TSource, TDestination>
    (this IMappingExpression<TSource, TDestination> expression, List<string> ignoreProperties = null)
        {
            if (ignoreProperties != null && ignoreProperties.Any())
            {
                var sourceType = typeof(TSource);
                var destinationType = typeof(TDestination);

                // 获取需要忽略的属性
                var destinationProperties = destinationType.GetProperties().Where(t => ignoreProperties.Contains(t.Name));

                foreach (var property in destinationProperties)
                {
                    //当源对象属性为null时，忽略目标对象属性
                    expression.ForMember(property.Name, opt => opt.Condition((src, dest, srcMember) => srcMember != null));
                }
            }

            // 通用忽略规则，例如忽略Id属性
            expression.ForMember("Id", opt => opt.Ignore());

            return expression;
        }
    }
}
