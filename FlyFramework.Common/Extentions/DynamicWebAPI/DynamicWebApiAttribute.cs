using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Common.Extentions.DynamicWebAPI
{
    /// <summary>
    /// 动态WebAPI特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class DynamicWebApiAttribute : Attribute
    {
    }
}
