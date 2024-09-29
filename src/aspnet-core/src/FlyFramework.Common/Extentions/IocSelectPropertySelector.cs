using Autofac.Core;

using FlyFramework.Attributes;

using System.Linq;
using System.Reflection;

namespace FlyFramework.Extentions
{
    /// <summary>
    /// 属性注入选择器
    /// </summary>
    public class IocSelectPropertySelector : IPropertySelector
    {
        public bool InjectProperty(PropertyInfo propertyInfo, object instance)
        {
            // 带有 AutowiredAttribute 特性的属性会进行属性注入
            return propertyInfo.CustomAttributes.Any(it => it.AttributeType == typeof(IocSelectAttribute));
        }
    }
}
