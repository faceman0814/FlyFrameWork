using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Common.Attributes
{
    /// <summary>
    /// 禁用工作单元属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class DisabledUnitOfWorkAttribute : Attribute
    {
        public readonly bool Disabled;

        public DisabledUnitOfWorkAttribute(bool disabled = true)
        {
            Disabled = disabled;
        }
    }
}
