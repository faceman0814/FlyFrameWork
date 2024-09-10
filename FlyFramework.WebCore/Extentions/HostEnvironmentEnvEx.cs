using Microsoft.Extensions.Hosting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.WebCore.Extentions
{
    public static class HostEnvironmentEnvEx
    {
        public static bool IsTesting(this IHostEnvironment hostEnvironment)
        {
            if (hostEnvironment.EnvironmentName == null)
            {
                throw new Exception("EnvironmentName is null");
            }
            return hostEnvironment.IsEnvironment(EnvironmentExs.Testing);
        }
    }
}
