using AutoMapper;

namespace GCT.MedPro.Application
{
    /// 映射扩展
    /// </summary>
    public static class AutoMapperExtensions
    {
        /// <summary>
        /// 忽略不需要映射的属性。
        /// </summary>
        public static IMappingExpression<TSource, TDestination> IgnoreNullSourceProperties<TSource, TDestination>
    (this IMappingExpression<TSource, TDestination> expression)
        {
            //var sourceType = typeof(TSource);
            //var destinationType = typeof(TDestination);

            //// 获取目标类型的所有属性
            //var destinationProperties = destinationType.GetProperties();

            //foreach (var property in destinationProperties)
            //{
            //    var sourceProperty = sourceType.GetProperty(property.Name);

            //    if (sourceProperty != null)
            //    {
            //        // 设置条件映射，当源属性为null时忽略该属性的映射
            //        expression.ForMember(property.Name, opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            //    }
            //}
            // 通用忽略规则，例如忽略Id属性
            expression.ForMember("Id", opt => opt.Ignore());

            return expression;
        }
    }
}
