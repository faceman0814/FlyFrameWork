using AutoMapper;

using Masuit.Tools;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Common.Attributes
{
    /// <summary>
    /// 用于标记 AutoMapper 映射时需要忽略的属性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class IgnoreMappingAttribute : Attribute
    {
    }

    /// <summary>
    /// 映射扩展
    /// </summary>
    public static class AutoMapperExtensions
    {
        /// <summary>
        /// 忽略不需要映射的属性。
        /// </summary>
        public static IMappingExpression<TSource, TDestination> IgnoreProperty<TSource, TDestination>
            (this IMappingExpression<TSource, TDestination> expression)
        {
            typeof(TSource).GetProperties()
                .Where(t => t.IsDefined(typeof(IgnoreMappingAttribute), true))
                .ForEach(t => expression.ForMember(t.Name, op => op.Ignore()));

            expression.ForMember("Id", op => op.Ignore());

            return expression;
        }
    }
}
