using Autofac.Core;

using System.Reflection;

namespace FlyFramework.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IocSelectAttribute : Attribute
    {

    }

    public class IoctAttributeSelector : IPropertySelector
    {
        public bool InjectProperty(PropertyInfo propertyInfo, object instance)
        {
            return propertyInfo.CustomAttributes.Any(A => A.AttributeType == typeof(IocSelectAttribute));
        }
    }
}
