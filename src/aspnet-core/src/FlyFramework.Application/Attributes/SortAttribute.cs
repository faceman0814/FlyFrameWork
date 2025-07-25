using System;

namespace FlyFramework.Attributes
{
    // 排序特性
    [AttributeUsage(AttributeTargets.Property)]
    public class SortAttribute : Attribute
    {
        public int Order { get; }

        public SortAttribute(int order)
        {
            Order = order;
        }
    }
}
