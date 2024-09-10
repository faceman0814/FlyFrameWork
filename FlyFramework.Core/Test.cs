using FlyFramework.Core.Extentions.DynamicWebAPI;

using Microsoft.AspNetCore.Mvc;

namespace FlyFramework.Core
{
    public class Test : IApplicationService
    {
        /// <summary>
        ///领域层注释测试接口
        /// </summary>
        /// <returns></returns>
        public string Hello()
        {
            return "Hello from Class1";
        }

        public string Get()
        {
            return "Get from Class1";
        }
    }
}
