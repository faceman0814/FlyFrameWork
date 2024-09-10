using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Application.Extentions.DynamicWebAPI
{
    /// <summary>
    /// 动态属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class DynamicWebApiAttribute : Attribute
    {
    }
}
