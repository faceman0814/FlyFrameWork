using JetBrains.Annotations;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NotNullAttribute = JetBrains.Annotations.NotNullAttribute;

namespace FlyFramework.Extentions
{
    public static class Check
    {
        [ContractAnnotation("value:null => halt")]
        public static T NotNull<T>(
            [System.Diagnostics.CodeAnalysis.NotNull] T? value,
            [InvokerParameterName][NotNull] string parameterName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }
    }
}
